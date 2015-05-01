# dynamic-mail

Sending email by using Web.config file

You could configure dynamic values in the dynamic_mail_values of the Web.config.
You could use a class to set these dynamic values as done in MailContrller : 

```C#
// Création d'un objet DynamicValues
DynamicValues dyn = new DynamicValues();
dyn.lastName = "Martin";
dyn.job = "Developer";
dyn.email = "test@test.com";
dyn.age = 45;

```

Or you could place the values in any database and retrieve it.