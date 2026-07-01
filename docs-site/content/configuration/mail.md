# Mail Configuration

Email sending driver and SMTP settings.

**File:** `src/API/config/mail.json`  
**Options class:** `MailOptions`

## Settings

| Key | Default | Description |
|-----|---------|-------------|
| `Mail:Driver` | `Log` | `Log` or `Smtp` |
| `Mail:Host` | `127.0.0.1` | SMTP server host |
| `Mail:Port` | `2525` | SMTP port |
| `Mail:UseStartTls` | `true` | STARTTLS on port 587 |
| `Mail:Username` | `""` | SMTP auth user |
| `Mail:Password` | `""` | SMTP auth password |
| `Mail:FromAddress` | `no-reply@template-net10.local` | Sender email |
| `Mail:FromName` | `Template-net10` | Sender display name |
| `Mail:TimeoutSeconds` | `30` | SMTP timeout |

## Drivers

| Driver | Behaviour |
|--------|-----------|
| `Log` | Writes rendered message to logger (default for local dev) |
| `Smtp` | Sends via MailKit SMTP |

## Production Override

```json
// config/Production/mail.json
{
  "Mail": {
    "Driver": "Smtp",
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "FROM_ENV_OR_SECRETS",
    "Password": "FROM_ENV_OR_SECRETS"
  }
}
```

## Related

- [Mail Overview](/docs/mail/overview)
- [Sending Email](/docs/mail/sending-email)
