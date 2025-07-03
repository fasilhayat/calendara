# Calendar REST API Service

This Calendar REST API service provides information about public holidays and weekends for Denmark, Sweden, Norway, and the European Union. The service supports querying holidays and weekends for a wide range of years (1975.01.01 - 2075.12.31) and uses a high-performance in-memory cache (Redis) to ensure fast access to holiday data.

---

## Features

- **Supported Countries**:
  - Denmark (`DK`)
  - Sweden (`SW`)
  - Norway (`NO`)
  - European Union (`EU`) (no year restrictions - unlimited)
- **Year Range**: 1975 to 2075
- **Caching**: Utilizes Redis for fast, in-memory data retrieval.
- **Empty Response Handling**: If no holidays are found for the requested country and date, the service will return an empty array (`[]`).

---

## API Endpoints

### 1. Get Holiday or Weekend Information
Retrieve detailed information about holidays or weekends for a specific country and date.

**Endpoint**:  
`https://[API-URL]/v1/calendar/holidayorweekend/{country}/{date}`

**Method**: `GET`

**Path Parameters**:
- `country` (required): The country code (`DK`, `SW`, `NO`, or `EU`).
- `date` (required): The specific date in `YYYY-MM-DD` format.

**Headers**:
- `X-API-KEY`: (required) Your API key for authenticating requests.

---

# Nager.Date Holiday Object Explained

```json
{
  "date": "2025-01-01T00:00:00",
  "localName": "Nytårsdag",
  "name": "New Year's Day",
  "countryCode": "DK",
  "fixed": false,
  "global": true,
  "counties": null,
  "launchYear": null,
  "types": [
    "Public"
  ]
}
```
### Field Descriptions
| Field |Type |	Description |
|:--------------|:---------------|:--------------|
|`date`|string (ISO 8601)|The date of the holiday in YYYY-MM-DD format.|
|`localName`|string|	The official/local name of the holiday in the country’s native language.|
|`name`|string|The English-translated or canonical name of the holiday.|
|`countryCode`|	string|	ISO 3166-1 alpha-2 country code (e.g., "ZA" for South Africa).|
|`fixed`  |	bool|	true if the holiday always occurs on the same calendar date (e.g., Jan 1); false if it varies.|
|`global` |	bool	true if this holiday is observed across the entire country; false if it's regional or partial.|
|`counties`	|string[] or null|	If not null, contains a list of region codes (e.g., state/province) where the holiday applies. null means it’s nationwide.|
|`launchYear` |	int or null|	The year the holiday was first officially recognized. null if unknown.|
|`type` |	string | The classification of the holiday, e.g., "Public", "Bank", "School", "Optional", or "Observance". |

---

### Success Responses:

#### Single Holiday or Weekend Example:
```json
{
  "date": "2025-01-04T00:00:00",
  "localName": "Weekend (Saturday)",
  "name": "Weekend (Saturday)",
  "countryCode": "DK",
  "fixed": false,
  "global": false,
  "counties": null,
  "launchYear": null,
  "types": [
    "Public",
    "Bank",
    "School"
  ]
}
```

#### Multiple Holidays Example:
```json
[
  {
    "date": "2024-01-01T00:00:00",
    "localName": "Nytårsdag",
    "name": "New Year's Day",
    "countryCode": "DK",
    "fixed": false,
    "global": true,
    "counties": null,
    "launchYear": null,
    "types": [
      "Public"
    ]
  },
  {
    "date": "2024-03-28T00:00:00",
    "localName": "Skærtorsdag",
    "name": "Maundy Thursday",
    "countryCode": "DK",
    "fixed": false,
    "global": true,
    "counties": null,
    "launchYear": null,
    "types": [
      "Public"
    ]
  },
  {
    "date": "2024-03-29T00:00:00",
    "localName": "Langfredag",
    "name": "Good Friday",
    "countryCode": "DK",
    "fixed": false,
    "global": true,
    "counties": null,
    "launchYear": null,
    "types": [
      "Public"
    ]
  }
]
```

#### Empty Response:
If no holidays or weekends are found, the service will return:
```json
[]
```

### Example Requests:
#### Using cURL:
```cmd
curl -X 'GET' \
  'https://[API-URL]]/v1/calendar/holidayorweekend/DK/2025-01-04' \
  -H 'accept: */*' \
  -H 'X-API-KEY: [API-KEY]'
```

#### HTTP Request Example:
```cmd
GET /v1/calendar/holidayorweekend/DK/2025-01-04 HTTP/1.1
Host: localhost:8080
accept: */*
X-API-KEY: [API-KEY]
```
