# WWW-Authenticate

![auth flow](content/basic-auth.svg)

tak działa dla każdego schematu uwierzytelniania

## Apache
```bash
podman run --rm httpd htpasswd -bn admin tajne123 > .htpasswd

podman run --rm httpd cat /usr/local/apache2/conf/httpd.conf > httpd.conf

cat << 'EOF' >> httpd.conf

<Directory "/usr/local/apache2/htdocs/private">
    AuthType Basic
    AuthName "Prywatne"
    AuthUserFile /usr/local/apache2/htdocs/.htpasswd
    AuthBasicAuthoritative On
    Require valid-user
</Directory>
EOF

podman run -d \
  --name apache \
  -p 8080:80 \
  -v "$PWD":/usr/local/apache2/htdocs/ \
  -v "$PWD/httpd.conf":/usr/local/apache2/conf/httpd.conf:ro \
  httpd
```

## Słownik
- resource authentication
- proxy authentication