﻿# TlsCertificateLoader Certbot sample without middleware
This is a sample that uses files created by [Certbot](https://certbot.eff.org/) to host an HTTP/3 website on Kestrel with HSTS, redirection of non-HTTPS traffic to HTTPS and redirection of "www." subdomain to the domain.

A [sample project using Certbot middleware](https://github.com/MarkCiliaVincenti/TlsCertificateLoader/tree/master/Samples/CertbotSampleUsingMiddleware) is available.

## Before first use
First edit the settings in Worker.cs. Then make sure that [Certbot](https://certbot.eff.org/) is set up. If you don't already have the certificates, use:

```
certbot certonly --standalone
```

You need to do this twice, once for the domain and once for the www subdomain.

If you want you can switch [Certbot](https://certbot.eff.org/) to use --webroot instead, which you can set to the path of the wwwroot folder.
