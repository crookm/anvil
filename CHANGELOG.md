# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v0.0.5] - 2025-03-14
### :bug: Bug Fixes
- [`69b035e`](https://github.com/crookm/anvil/commit/69b035e4a5c8c7745086522ac825ef640743dbc5) - overly-lenient pages domain parsing allowing unlimited valid domains for a single repo *(commit by [@crookm](https://github.com/crookm))*

### :white_check_mark: Tests
- [`6f2b13c`](https://github.com/crookm/anvil/commit/6f2b13c6ce0e31a9bd836f118bd62487b9fcfaaa) - pages domain parsing *(commit by [@crookm](https://github.com/crookm))*

### :wrench: Chores
- [`e86531e`](https://github.com/crookm/anvil/commit/e86531e243fa3de395c3f65ed68e950f6d454fb6) - move dockerfile to root of the repository *(commit by [@crookm](https://github.com/crookm))*
- [`a0b571a`](https://github.com/crookm/anvil/commit/a0b571ac3565461b41c6873ed83f078e24a9ed29) - monitor docker with dependabot *(commit by [@crookm](https://github.com/crookm))*
- [`44834bf`](https://github.com/crookm/anvil/commit/44834bfc4ea9d4c782b48881a9c9d02cc7207f16) - setup unit test project *(commit by [@crookm](https://github.com/crookm))*


## [v0.0.4] - 2025-03-01
### :sparkles: New Features
- [`667c1a0`](https://github.com/crookm/anvil/commit/667c1a0f5df9887066dc5457061ec829b86bb483) - internal endpoint to validate pages domains *(commit by [@crookm](https://github.com/crookm))*
- [`55da32e`](https://github.com/crookm/anvil/commit/55da32e6a4c8c8da4e62e410fceb3c42ace17ad7) - allow overriding the host header of source provider api requests *(commit by [@crookm](https://github.com/crookm))*
- [`e7f5d12`](https://github.com/crookm/anvil/commit/e7f5d12df59fb8fa41ae8a9d0204f8c08de06896) - allow ignoring TLS errors with source provider APIs *(commit by [@crookm](https://github.com/crookm))*

### :bug: Bug Fixes
- [`239c612`](https://github.com/crookm/anvil/commit/239c612796dc5f3f5fb1c220d353bcd631c144be) - allow unrelated txt records with the same name when parsing custom domains into repos *(commit by [@crookm](https://github.com/crookm))*

### :wrench: Chores
- [`874a908`](https://github.com/crookm/anvil/commit/874a908931e59ba9779076832ab98084a6d7b050) - enable .net user secrets *(commit by [@crookm](https://github.com/crookm))*


## [v0.0.3] - 2025-02-23
### :bug: Bug Fixes
- [`b6ba29a`](https://github.com/crookm/anvil/commit/b6ba29a02697f0740fa3fe0885f383aa9c034857) - missing resources falling back to the index document *(commit by [@crookm](https://github.com/crookm))*


## [v0.0.2] - 2025-02-23
### :bug: Bug Fixes
- [`ee5d373`](https://github.com/crookm/anvil/commit/ee5d373a1082ca697cd3f5620a5db214b1db0617) - incorrect value for custom domain check *(commit by [@crookm](https://github.com/crookm))*


## [v0.0.1] - 2025-02-23
### :sparkles: New Features
- [`c780d62`](https://github.com/crookm/anvil/commit/c780d624f18d96c6d9adebce8f162a9554a6efb2) - initial pages implementation *(commit by [@crookm](https://github.com/crookm))*
- [`9f9aa9a`](https://github.com/crookm/anvil/commit/9f9aa9a3d6d8e42cc69c2fbd6f7a899c6745d02a) - docker support *(commit by [@crookm](https://github.com/crookm))*

### :wrench: Chores
- [`b1dba26`](https://github.com/crookm/anvil/commit/b1dba263873c330a386d61a723f6ea070abed77d) - set assembly name given the runtime identifier *(commit by [@crookm](https://github.com/crookm))*

[v0.0.1]: https://github.com/crookm/anvil/compare/v0.0.0...v0.0.1
[v0.0.2]: https://github.com/crookm/anvil/compare/v0.0.1...v0.0.2
[v0.0.3]: https://github.com/crookm/anvil/compare/v0.0.2...v0.0.3
[v0.0.4]: https://github.com/crookm/anvil/compare/v0.0.3...v0.0.4
[v0.0.5]: https://github.com/crookm/anvil/compare/v0.0.4...v0.0.5
