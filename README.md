This project demonstrates how update an embedded subdocument in MongoDB.

NOTE: This solution was provided by Yong Shun and will update the first
record found that matches the filters. So you cannot use it if multiple
matches are found... but there shouldn't be multiple matches in the match 
criteria.

https://stackoverflow.com/questions/79786685/mongodb-net-updating-embedded-document-in-list-with-filters-based-on-parent-a


UPDATE: How to use the ID of a doc with the ArrayFilterDefinition 

	UpdateResult updateResult = collection.UpdateOne(combinedFilter, updateDefinition,
		new UpdateOptions
		{
			ArrayFilters = new ArrayFilterDefinition[]
			{
				new BsonDocumentArrayFilterDefinition<Player>
				(
				   // new BsonDocument("p.PlayerName", "Greg")  // this works
				  // new BsonDocument("p.Id", BsonValue.Create(GregsIdAsString))  // this does NOT work
				  new BsonDocument("p.Id", GregsIdAsString) // this also does not work.
				)
			}
		});