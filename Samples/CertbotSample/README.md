# TlsCertificateLoader Full Sample
This is a sample that uses files created by Certbot to host an HTTP/3 website on Kestrel with HSTS, redirection of non-HTTPS traffic to HTTPS and redirection of "www." subdomain to the domain.

## Before first use
First edit the settings in Worker.cs. Then make sure that Certbot is set up. If you don't already have the certificates, use:

```
certbot certonly --standalone
```

You need to do this twice, once for the domain and once for the www subdomain.

If you want you can switch Certbot to use --webroot instead, which you can set to the path of the wwwroot folder.