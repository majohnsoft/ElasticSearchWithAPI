# ElasticSearchWithAPI
ElasticSearch is an incredibly popular search technology. This project describe the effective use of elastic search with filters to fetch information from AWS cluster. 

# Smart Apartment Data Assessment
# Elasticsearch with autocomplete functionality
# Developed by Johnson Madumere

Building Autocomplete API with Completion Suggester in ASP.NET 5

Link to my video explaining the implementation: https://johnsonmadumere.com/Uploads/Video.html

# IMPLEMENTATION
I used the Layered Architecture Pattern also known as multi-layered or tiered architecture, or n-tier architecture. 
This architecture pattern allows for easy communication and makes it easy to implement a separation of concerns. I classified it into 4 distinct layers
1. The Presentation Layer => smart.web
2. The Service Layer => 
3. The Business Layer => smart.business and smart.Object
4. The Data Layer => smart.seed

The components in the presentation layer deal only with presentation logic, whereas components residing in the business layer deal only with business logic. This type of component classification makes it easy for me to build effective responsibility models into my architecture, and also makes it easy to develop, test, and maintain applications using this architecture pattern due to well-defined component interfaces and limited component scope.

The shared-services layer to my architecture contains API endpoints accessed by the presentation layer (e.g., Get Market region and search for apartment or management companies. Creating a services layer was a good idea in this case because architecturally it restricts access to the presentation layer.  

# KEY FEATURES 
1. custom exception handler to catch any exceptions
2. Enable Cross-Origin Requests (CORS)
3. Dependency Injection
4. Implemented some of the SOLID Princples. see below
	i. S: Single Responsibility Principle (SRP)
	ii. O: Open/Closed Principle
	iii. I: Interface Segregation Principle (ISP)
	iv. D: Dependency Inversion Principle
5. A generic/custom API Response object
6. Unit Testing which test the business layer using different test cases
7. Integration Testing which test the web api controllers
8. Use of custom analyzer, tokenizer and filters - Elasticsearch


The API makes use of the HTTP POST Method for searching and the HTTP GET Method for retrieving market regions

# REQUEST AND RESPONSE SEARCH JSON OBJECT
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
 
 # SAMPLE RESPONSE TO GET MARKET REGION
	[
	      	{
		  "market": "Hillside Ranch at the Cottages",
		  "state": "TX"
		},
		{
		  "market": "Austin",
		  "state": "TX"
		},
        ]


NOTE: Swagger Localhost URL: https://localhost:5363/swagger/index.html

