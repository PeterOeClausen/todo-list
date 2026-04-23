# TodoList.Api.Tests

Integration test suite for TodoList.Api.

Uses:

- Microsoft.AspNetCore.Mvc.Testing to spin up the API in memory.
- Respawn (Library by Jimmy Bogard) to take a database snapshot after database migrations, to revert to between tests for a fast clean and empty database between tests.
- Testcontainers.MsSql - TestContainers are lightweight containers made for testing. I use it to spin up sql databases in Docker for integration testing.
- TodoList.Api.Client project to call the API and get typed results back - TodoList.Api.Client can be published as a NuGet package to other APIs to be used in eg. a MicroService architecture.

Needs Docker Engine to be running in order to run tests. Runs in about 13.7 seconds on an M4 Macbook Pro.
