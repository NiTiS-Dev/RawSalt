# Logging
This directory contains all classes relative to the logging system in RawSalt engine.

## Plans
Remove root static `Log` class, and replace it with instantiable `Logger`.

```csharp
// Get root logger
var logger = Logger.Create();

logger.Info("Hello World");
// [23:59:59 INF] Hello World
```

All logger (or all loggers created by same factory) will share one logging thread