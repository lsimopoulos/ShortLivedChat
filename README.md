# ShortLivedChat

In order to make this sample run, add a certificate(yourceerrt.pfx) in the solution and set the  property "Copy to Output Directory" to "Always".Then replace the line 26 on the Startup.cs(in the server solution) 
  .AddSigningCredential(Cert.Get("thecert.pfx", "somepassword")); with your certificate name and password.
  
  TODO: write a proper readme 
