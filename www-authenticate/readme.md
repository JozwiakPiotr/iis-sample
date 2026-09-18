# WWW-Authenticate

![auth flow](content/basic-auth.svg)

tak działa dla każdego schematu uwierzytelniania

## Apache

Uruchom poniższe polecenia z katalogu `www-authenticate/httpd`:

```bash
mkdir -p private

cat << EOF > private/index.html
<!DOCTYPE html>
<html>
    <meta charset="utf-8">
</html>
<body>
    To jest chroniony zasób Apache.
</body>
EOF

podman run --rm httpd htpasswd -nb admin tajne123 > .htpasswd

podman rm -f apache 2>/dev/null || true
podman run -d \
  --name apache \
  -p 8080:80 \
  -v "$PWD/private":/usr/local/apache2/htdocs/private:ro,Z \
  -v "$PWD/.htpasswd":/usr/local/apache2/conf/.htpasswd:ro,Z \
  -v "$PWD/httpd.conf":/usr/local/apache2/conf/httpd.conf:ro,Z \
  httpd

curl -i http://localhost:8080/private/
curl -i -u admin:tajne123 http://localhost:8080/private/
```

Pierwsze żądanie powinno zwrócić `401 Unauthorized` i nagłówek `WWW-Authenticate`,
a drugie `200 OK`. Hasło `tajne123` jest przykładowe; zmień je przed użyciem poza lokalnym testem.

## Słownik
- resource authentication
- proxy authentication