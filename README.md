![Logo](./assets/shr-logo-preview.png)

# shr │ [![Build Status](https://github.com/haqa-ru/shr/actions/workflows/actions.yml/badge.svg?branch=main&event=push)](https://github.com/haqa-ru/shr/actions/workflows/actions.yml)

Instant sharing utility web interface designed for _programmatic_ or _manual_ use.

Originally started as a follow-up to [code.re](https://web.archive.org/web/20200304041752/http://code.re/), we will now pursue the natural extension of this project, moving from _text-sharing_ to _file-sharing_.

## Examples

### Upload

```bash
curl -T ./example.txt https://shr.haqa.ru/api/share/example.txt
```

### Download

```bash
curl https://shr.haqa.ru/api/share/EXML/content
```

## Documentation

**[Specification](https://shr.haqa.ru/api/openapi.yaml)**

## Licence

**shr** source code is licensed under the [MIT Licence](https://opensource.org/licenses/MIT) as stated in the [License File](./LICENCE).
