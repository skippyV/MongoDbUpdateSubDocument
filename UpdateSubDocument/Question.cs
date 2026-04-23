using System.Diagnostics.CodeAnalysis;

namespace UpdateSubDocument
{
    public class Question
    {
        // Not using a BSON ObjectId for this object.
        [SetsRequiredMembers]
        public Question(int num)
        {
            Number = num;
        }

        public required int Number { get; init; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public List<string> ImageFileIds { get; set; } = [];
        /// <summary>
        /// Returns True if the string was added.
        /// Returns False if the string was in the List.
        /// </summary>
        /// <param name="imageFileReference"></param>
        /// <exception cref="ArgumentException"></exception>
        public bool AddImageFileReference(string imageFileReference)
        {
            if (!string.IsNullOrEmpty(imageFileReference))
            {
                if (ImageFileIds.Contains(imageFileReference))
                {
                    return false;
                }
                else
                {
                    ImageFileIds.Add(imageFileReference);
                    return true;
                }
            }
            else
            {
                throw new ArgumentException("Parameter cannot not be null or empty string");
            }
        }

        public bool RemoveImageFileReference(string imageFileReference)
        {
            if (!string.IsNullOrEmpty(imageFileReference))
            {
                if (ImageFileIds.Contains(imageFileReference))
                {
                    ImageFileIds.Remove(imageFileReference);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new ArgumentException("Parameter cannot not be null or empty string");
            }
        }
    }

    public enum QuestionType { basic = 0, poll, advanced }

    public static class QuestionHelper
    {
        public static Question CreateQuestion(List<Question> list)
        {
            if (list is null)
            {
                throw new ArgumentException("Parameter cannot be null", nameof(QuestionHelper));
            }

            int questionNumber = list.Count == 0 ? 1 : list.Max(x => x.Number) + 1;

            return new Question(questionNumber);
        }

    }
}


