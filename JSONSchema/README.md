## JSONSchema Validator
This is a simple tool to validate JSON objects/files against JSON schemas.
It uses [NJsonSchema](https://github.com/RicoSuter/NJsonSchema) as the underlying library for the JSON schema validation so it supports whatever schemas it supports(draft v4+).


### Build
The build here is a bit special since I didn't want to waste too much time pushing the `NJsonSchema` dependency as part of the build so I manually added the required DLLs to the `.nupkg`.