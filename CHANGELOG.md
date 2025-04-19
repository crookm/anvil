# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v0.1.0] - 2025-04-19
### :bug: Bug fixes
- [`b29e4be`](https://enduringtech.dev/matt/anvil/commit/b29e4bec40b79b22f5402bb74eafe3c5ddd30009) - *forgejo*: trim leading slashes from repo file requests *(commit by [@matt](https://enduringtech.dev/matt))*

### :white_check_mark: Tests
- [`35feccd`](https://enduringtech.dev/matt/anvil/commit/35feccd1c1dff32f4451a62ad4a6163c381007d0) - txt record querying *(commit by [@matt](https://enduringtech.dev/matt))*
- [`cb66914`](https://enduringtech.dev/matt/anvil/commit/cb669143954e9478bcb8a4e70000301eb7c7fa48) - cname record querying *(commit by [@matt](https://enduringtech.dev/matt))*
- [`37df4f7`](https://enduringtech.dev/matt/anvil/commit/37df4f7d8557f3f50659ad509baaebd872d0a122) - mimetype parsing from request path *(commit by [@matt](https://enduringtech.dev/matt))*

### :wrench: Chores
- [`9643dd2`](https://enduringtech.dev/matt/anvil/commit/9643dd29c4f49bd400805a8b01bf805d67c7edf7) - name all steps of pipeline *(commit by [@matt](https://enduringtech.dev/matt))*
- [`caa271b`](https://enduringtech.dev/matt/anvil/commit/caa271b285b0ce3138416e6c3fa59024b331b1f5) - update pipelines for source migration *(commit by [@matt](https://enduringtech.dev/matt))*
- [`d28f55f`](https://enduringtech.dev/matt/anvil/commit/d28f55f7c3cc6ab497d51d20226ea6d1c4d734f5) - correct logger type in cname lookup query handler *(commit by [@matt](https://enduringtech.dev/matt))*
- [`dbe62d9`](https://enduringtech.dev/matt/anvil/commit/dbe62d9f50726b6b38552c463a33cd3edd71ece5) - flatten test project directory structure *(commit by [@matt](https://enduringtech.dev/matt))*
- [`465d6bb`](https://enduringtech.dev/matt/anvil/commit/465d6bb62d8dc0f6127300bed11e41f5e485e7bf) - exclude test code from dockerfile build scope *(commit by [@matt](https://enduringtech.dev/matt))*


## [v0.0.5] - 2025-03-14
### :bug: Bug Fixes
- [`69b035e`](https://enduringtech.dev/matt/anvil/commit/69b035e4a5c8c7745086522ac825ef640743dbc5) - overly-lenient pages domain parsing allowing unlimited valid domains for a single repo *(commit by [@matt](https://enduringtech.dev/matt))*

### :white_check_mark: Tests
- [`6f2b13c`](https://enduringtech.dev/matt/anvil/commit/6f2b13c6ce0e31a9bd836f118bd62487b9fcfaaa) - pages domain parsing *(commit by [@matt](https://enduringtech.dev/matt))*

### :wrench: Chores
- [`e86531e`](https://enduringtech.dev/matt/anvil/commit/e86531e243fa3de395c3f65ed68e950f6d454fb6) - move dockerfile to root of the repository *(commit by [@matt](https://enduringtech.dev/matt))*
- [`a0b571a`](https://enduringtech.dev/matt/anvil/commit/a0b571ac3565461b41c6873ed83f078e24a9ed29) - monitor docker with dependabot *(commit by [@matt](https://enduringtech.dev/matt))*
- [`44834bf`](https://enduringtech.dev/matt/anvil/commit/44834bfc4ea9d4c782b48881a9c9d02cc7207f16) - setup unit test project *(commit by [@matt](https://enduringtech.dev/matt))*


## [v0.0.4] - 2025-03-01
### :sparkles: New Features
- [`667c1a0`](https://enduringtech.dev/matt/anvil/commit/667c1a0f5df9887066dc5457061ec829b86bb483) - internal endpoint to validate pages domains *(commit by [@matt](https://enduringtech.dev/matt))*
- [`55da32e`](https://enduringtech.dev/matt/anvil/commit/55da32e6a4c8c8da4e62e410fceb3c42ace17ad7) - allow overriding the host header of source provider api requests *(commit by [@matt](https://enduringtech.dev/matt))*
- [`e7f5d12`](https://enduringtech.dev/matt/anvil/commit/e7f5d12df59fb8fa41ae8a9d0204f8c08de06896) - allow ignoring TLS errors with source provider APIs *(commit by [@matt](https://enduringtech.dev/matt))*

### :bug: Bug Fixes
- [`239c612`](https://enduringtech.dev/matt/anvil/commit/239c612796dc5f3f5fb1c220d353bcd631c144be) - allow unrelated txt records with the same name when parsing custom domains into repos *(commit by [@matt](https://enduringtech.dev/matt))*

### :wrench: Chores
- [`874a908`](https://enduringtech.dev/matt/anvil/commit/874a908931e59ba9779076832ab98084a6d7b050) - enable .net user secrets *(commit by [@matt](https://enduringtech.dev/matt))*


## [v0.0.3] - 2025-02-23
### :bug: Bug Fixes
- [`b6ba29a`](https://enduringtech.dev/matt/anvil/commit/b6ba29a02697f0740fa3fe0885f383aa9c034857) - missing resources falling back to the index document *(commit by [@matt](https://enduringtech.dev/matt))*


## [v0.0.2] - 2025-02-23
### :bug: Bug Fixes
- [`ee5d373`](https://enduringtech.dev/matt/anvil/commit/ee5d373a1082ca697cd3f5620a5db214b1db0617) - incorrect value for custom domain check *(commit by [@matt](https://enduringtech.dev/matt))*


## [v0.0.1] - 2025-02-23
### :sparkles: New Features
- [`c780d62`](https://enduringtech.dev/matt/anvil/commit/c780d624f18d96c6d9adebce8f162a9554a6efb2) - initial pages implementation *(commit by [@matt](https://enduringtech.dev/matt))*
- [`9f9aa9a`](https://enduringtech.dev/matt/anvil/commit/9f9aa9a3d6d8e42cc69c2fbd6f7a899c6745d02a) - docker support *(commit by [@matt](https://enduringtech.dev/matt))*

### :wrench: Chores
- [`b1dba26`](https://enduringtech.dev/matt/anvil/commit/b1dba263873c330a386d61a723f6ea070abed77d) - set assembly name given the runtime identifier *(commit by [@matt](https://enduringtech.dev/matt))*


[v0.0.1]: https://enduringtech.dev/matt/anvil/compare/v0.0.0...v0.0.1
[v0.0.2]: https://enduringtech.dev/matt/anvil/compare/v0.0.1...v0.0.2
[v0.0.3]: https://enduringtech.dev/matt/anvil/compare/v0.0.2...v0.0.3
[v0.0.4]: https://enduringtech.dev/matt/anvil/compare/v0.0.3...v0.0.4
[v0.0.5]: https://enduringtech.dev/matt/anvil/compare/v0.0.4...v0.0.5
[v0.1.0]: https://enduringtech.dev/matt/anvil/compare/v0.0.5...v0.1.0
