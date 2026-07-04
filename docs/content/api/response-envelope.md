# Response Envelope

Every API endpoint returns a consistent JSON envelope so clients can handle success and failure uniformly.

## ApiResponseDto\<T\>

Used for single-object responses:

```json
{
  "isSuccess": true,
  "data": { "id": 1, "email": "admin@template-net10.local" },
  "message": null,
  "errors": null
}
```

On failure:

```json
{
  "isSuccess": false,
  "data": null,
  "message": "User not found.",
  "errors": null
}
```

Handlers construct responses with:

```csharp
ApiResponseDto<UserDto>.Success(dto);
ApiResponseDto<UserDto>.Failed(_localization.GetMessage(Resource.UserNotFound));
```

## PagedApiResponseDto\<T\>

Used for paginated lists. Extends the envelope with a `metaData` block:

```json
{
  "isSuccess": true,
  "data": [ { "id": 1, "nameEn": "Admin" } ],
  "metaData": {
    "resultSet": { "count": 137, "limit": 20, "offset": 0 }
  }
}
```

## MessageDto

Used for operations that return only a message (logout, assign roles, delete):

```json
{
  "isSuccess": true,
  "data": { "message": "Roles assigned successfully." }
}
```

## Controller Pattern

Controllers inherit `ApiControllerBase` and return MediatR results directly:

```csharp
[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponseDto<UserDto>))]
public async Task<ActionResult<ApiResponseDto<UserDto>>> GetById([FromRoute] int id)
    => await Sender.Send(new GetUserByIdQuery { Id = id });
```

## Related

- [Pagination](/docs/api/pagination)
- [Exception Handling](/docs/api/exception-handling)
