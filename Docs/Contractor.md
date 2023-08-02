# Buber Contractor API

- [LinkUp API](#linkup-api)
  - [Create Contractor](#create-contractor)
    - [Create Contractor Request](#create-contractor-request)
    - [Create Contractor Response](#create-Contractor-response)
  - [Get Contractor](#get-Contractor)
    - [Get Contractor Request](#get-Contractor-request)
    - [Get Contractor Response](#get-Contractor-response)
  - [Update Contractor](#update-Contractor)
    - [Update Contractor Request](#update-Contractor-request)
    - [Update Contractor Response](#update-Contractor-response)
  - [Delete Contractor](#delete-Contractor)
    - [Delete Contractor Request](#delete-Contractor-request)
    - [Delete Contractor Response](#delete-Contractor-response)

## Create Contractor

### Create Contractor Request

```js
POST /Contractors
```

```json
{
    "name": "Juan Paul 2",
    "email": "JP2@vatican.it",
    "password": "JeszczeJak2137"
}
```

### Create Contractor Response

```js
201 Created
```

```yml
Location: {{host}}/Contractors/{{id}}
```

```json
{
    "id": "21372137-2137-2137-2137-213721372137",
    "name": "Juan Paul 2",
    "email": "JP2@vatican.it",
    "password": "JeszczeJak2137"
}
```

## Get Contractor

### Get Contractor Request

```js
GET /Contractors/{{id}}
```

### Get Contractor Response

```js
200 Ok
```

```json
{
    "id": "21372137-2137-2137-2137-213721372137",
    "name": "Juan Paul 2",
    "email": "JP2@vatican.it",
    "password": "JeszczeJak2137"
}
```

## Update Contractor

### Update Contractor Request

```js
PUT /Contractors/{{id}}
```

```json
{
    "name": "John Player II",
    "email": "JPII@vatican.it",
    "password": "TakJakPanJezusPowiedzial"
}
```

### Update Contractor Response

```js
204 No Content
```

or

```js
201 Created
```

```yml
Location: {{host}}/Contractors/{{id}}
```

## Delete Contractor

### Delete Contractor Request

```js
DELETE /Contractors/{{id}}
```

### Delete Contractor Response

```js
204 No Content
```