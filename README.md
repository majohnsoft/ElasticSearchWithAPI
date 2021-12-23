# ElasticSearchWithAPI
Search is a critical component of our Big Data infrastructure. ElasticSearch is an incredibly popular search technology. This project describe the effective use of elastic search with filters to fetch information from AWS cluster. 


# Smart Apartment Data Assessment
# Elasticsearch with autocomplete functionality
# Developed by Johnson Madumere

Building Autocomplete API with Completion Suggester in ASP.NET 5

Link to my video explaining the implementation: https://johnsonmadumere.com/Uploads/Video.html

# IMPLEMENTATION
I used the Layered Architecture Pattern also known as multi-layered or tiered architecture, or n-tier architecture was used. 
This architecture pattern allows for easy communication and ease of undertanding. I classified it into 4 distinct layers
1. The Presentation Layer => smart.web
2. The service Layer => smart.API
3. The Business Layer => smart.business and smart.Object
4. The Data Layer => smart.seed

The Presentation layer speak on the customer or user experience and it calls the smart.API project to either get all market region or search for properties and management companies
The Service layer: The api services such as get market region or search (auto-complete search) is done via the service layer. This communicate directly to the Business and Business Object Layer
	SAMPLE SEARCH REQUEST JSON
		{
		  "indexName": "",
		  "keyword": "Cottage",
		  "limit": 50,
		  "market": [
			"Austin"
		  ]
		}

	SAMPLE SEARCH RESPONSE JSON
	{
      "payload": [
        {
          "name": "Hillside Ranch at the Cottages",
          "market": "Austin",
          "state": "TX",
          "isApartment": true,
          "score": 6
        },
        {
          "name": "Cottages at San Marcos",
          "market": "Austin",
          "state": "TX",
          "isApartment": true,
          "score": 6
        },
      ],
      "success": true,
      "description": "Successful",
      "responseCode": 200
    }
    NOTE: Swagger Localhost URL: https://localhost:5363/swagger/index.html

The Business Layer: This is where the main implementation is done. It contains three classes namely, ManagementServices, MarketRegionService and PropertyServices class
The Data Layer: This project handles the seeding creation of index and also uploading the data to AWS Elasticsearch cluster.
it to the respective index that was created

Note: There is also a unit testing and integration testing to ensure the process/implementation are well tested using the scope that was defined.
