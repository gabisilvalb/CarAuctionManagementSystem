# Car Auction Management System

A comprehensive auction management system built with C# and ASP.NET Core that handles different types of vehicles (Sedans, SUVs, Hatchbacks and Trucks) with full auction lifecycle management.

## Features

- **Vehicle Management**: Add, update, delete, and search vehicles by type, manufacturer, model, or year
- **Auction Lifecycle**: Create, start, and close auctions with proper state management
- **Bidding System**: Place bids with validation and real-time highest bid tracking
- **Bidder Management**: Register bidders and track their auction participation
- **Comprehensive Validation**: Multi-layer validation with detailed error messages
- **RESTful API**: Clean, intuitive endpoints following REST conventions

## System Architecture

The system follows a clean layered architecture with clear separation of concerns:

```
┌─────────────────┐
│   API Layer     │  (Handlers - HTTP request/response)
├─────────────────┤
│  Service Layer  │  (Business Logic)
├─────────────────┤
│ Repository      │  (Data Access)
├─────────────────┤
│  Domain Models  │  (Entities & Enums)
└─────────────────┘
```

## Vehicle Types Supported

| Vehicle Type | Specific Attributes |
|--------------|-------------------|
| **Sedan** | Number of doors |
| **Hatchback** | Number of doors |
| **SUV** | Number of seats |
| **Truck** | Load capacity |

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

### Running the Application

1. **Clone the repository**
```bash
git clone https://github.com/gabisilvalb/CarAuctionManagementSystem.git
cd CarAuctionManagementSystem
```

2. **Build and run**
```bash
dotnet build
dotnet run --project CarAuctionManagementSystem
```

3. **Access the API**
- API runs on: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Running Tests
```bash
dotnet test
```

## API Endpoints

### Vehicles
```http
GET    /vehicles                    # Search vehicles
POST   /vehicles                    # Add new vehicle
GET    /vehicles/{id}               # Get vehicle by ID
PUT    /vehicles/update             # Update vehicle
DELETE /vehicles/{id}               # Delete vehicle
```

### Auctions
```http
POST   /auctions                    # Create auction
POST   /auctions/start              # Start auction
GET    /auctions/all                # Get all auctions
GET    /auctions/onGoing            # Get ongoing auctions
GET    /auctions/closed             # Get closed auctions
GET    /auctions/{id}               # Get auction by ID
POST   /auctions/bid                # Place bid
POST   /auctions/close              # Close auction
GET    /auctions/{id}/bids          # Get auction bids
```

### Bidders
```http
POST   /bidders                     # Create bidder
DELETE /bidders/{id}                # Delete bidder
GET    /bidders/{id}                # Get bidder details
GET    /bidders/{id}/auctions       # Get bidder's auctions
```

## Design Decisions and Assumptions

### Architecture Design

#### Layered Architecture
The system implements a 4-layer architecture:

1. **API Layer (Handlers)**: Handles HTTP requests, validation, and response formatting
2. **Service Layer**: Contains business logic 
3. **Repository Layer**: Manages data access and persistence logic
4. **Domain Layer**: Contains core business models and enums

This separation provides clear boundaries between concerns, making the code more maintainable and testable.

### Domain Model Design

#### **Vehicle Hierarchy**
Implemented vehicle types using inheritance with a base `Vehicle` class and specialized subclasses:

```
Vehicle (Base Class)
├── Sedan (NumberOfDoors)
├── Hatchback (NumberOfDoors) 
├── SUV (NumberOfSeats)
└── Truck (LoadCapacity)
```

- **Polymorphism**: Allows uniform treatment of different vehicle types
- **Type Safety**: Each vehicle type has its specific attributes
- **Extensibility**: Easy to add new vehicle types without modifying existing code
- **Data Integrity**: Enforces that each vehicle type has appropriate attributes

#### **Entity Identity Management**
All entities use `Guid` for unique identification with protected setters:
- `Vehicle.Id`, `Auction.Id`, `Bidder.Id`, `Bid.Id`

- **Uniqueness**: GUIDs eliminate collision risks
- **Encapsulation**: Protected setters prevent external ID manipulation (also used fortesting)
- **Immutability**: IDs are set once during creation

### Business Logic Implementation

#### **Auction State Management**
Implemented auction status using an enumeration:

State Transition Rules:
- `NotStarted` → `OnGoing` (via StartAuction)
- `OnGoing` → `Closed` (via CloseAuction)
- No backward transitions allowed

#### **Bidding Rules & Validation**

Implemented Business Rules:
1. Bids must exceed the vehicle's starting price
2. New bids must be higher than current highest bid
3. Bidding only allowed during ongoing auctions
4. Bidders must be registered before placing bids

### Data Management Strategy

#### **In-Memory Storage**
Used `List<T>` collections for data persistence:
- `List<Vehicle> _vehicles`
- `List<Auction> _auctions` 
- `List<Bidder> _bidders`

- Simplicity: No database setup required for demonstration
- Performance: Fast operations for small datasets

Trade-offs:
- Data is not persistent across application restarts

### Validation Strategy

#### **Multi-Layer Validation**
Implemented validation at multiple levels:

1. **Input Validation**: FluentValidation for request DTOs
2. **Business Rule Validation**: Service layer validation
3. **Entity Validation**: Domain model constraints

### Error Handling Architecture

#### **Custom Exception Hierarchy**
Created specific exception types for different error conditions.

#### **Global Exception Middleware**
Centralized exception handling with appropriate HTTP status codes for example:

```csharp
var statusCode = exception switch
{
    VehicleNotFoundException => HttpStatusCode.NotFound,
    BidAmountTooLowException => HttpStatusCode.BadRequest,
    AuctionNotActiveException => HttpStatusCode.Conflict
};
```
- Uniform error response format
- Single point for error handling logic
- Clear, actionable error messages

### API Design Decisions

#### **RESTful Endpoint Design**
Implemented REST-compliant endpoints following standard conventions.

#### **DTO Pattern Implementation**
Separate DTOs for requests and responses

**Rationale**:
- API contracts independent of domain models
- Enables API evolution without breaking changes

## Testing Strategy

### Comprehensive Unit Test Coverage
Implemented extensive unit tests covering:

1. **Service Layer Tests**: Business logic validation
2. **Handler Tests**: Request/response processing  
3. **Validation Tests**: Input validation rules

Testing Patterns Used:
- **AAA Pattern**: Arrange, Act, Assert
- **Mocking**: Using Moq framework for dependencies
- **FluentAssertions**: Readable assertion syntax

### Test Examples
- **Happy Path**: Valid operations return expected results
- **Error Scenarios**: Invalid inputs throw appropriate exceptions
- **Business Rules**: Auction lifecycle and bidding rules enforced
- **Edge Cases**: Boundary conditions and null handling

## Key Assumptions Made

### Business Assumptions
1. **Single Vehicle Per Auction**: Each auction is for exactly one vehicle
2. **Sequential Auction States**: Auctions follow a linear state progression NotStarted -> OnGOing -> Closed
3. **Bid Increment Rules**: Any amount higher than current highest is valid
4. **Bidder Registration**: Bidders must be pre-registered before bidding
5. **Auction Finality**: Closed auctions cannot be reopened

### Technical Assumptions
1. **In-Memory Storage**: Acceptable for demonstration purposes
2. **Single Instance**: No concurrent access considerations needed
3. **UTC Timestamps**: All times in UTC format
4. **GUID Uniqueness**: System-generated GUIDs are unique

### Some Data Assumptions
1. **Vehicle Years**: Valid range from 1886 (first vehicle) to current year
2. **Monetary Values**: Decimal type sufficient for bid amounts
3. **Email Uniqueness**: Each bidder has a unique email address

## Future Enhancement Opportunities

### Scalability Improvements
- **Database Integration**: Replace in-memory storage with persistent database
- **Caching Layer**: Add Redis for frequently accessed data

### Feature Enhancements
- **Reserve Prices**: Minimum acceptable bid amounts
- **Bid History**: Detailed bidding timeline
- **Authentication**: User authentication and authorization

### Operational Improvements
- **Logging**: Comprehensive application logging
- **Monitoring**: Health checks and metrics

## Technologies Used

- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Validation**: FluentValidation
- **Testing**: Moq, FluentAssertions
- **Documentation**: Swagger/OpenAPI
- **Architecture**: Minimal API, Clean Architecture, Repository Pattern, Dependency Injection