# Anvil Pages Server

[![GitHub Actions status of CI workflow](https://img.shields.io/github/actions/workflow/status/crookm/anvil/ci.yml?label=ci)](https://github.com/crookm/anvil/actions/workflows/ci.yml)
[![GitHub Actions status of CI workflow](https://img.shields.io/github/actions/workflow/status/crookm/anvil/release.yml?label=release)](https://github.com/crookm/anvil/actions/workflows/release.yml)
[![Latest release version](https://img.shields.io/github/v/release/crookm/anvil)](https://github.com/crookm/anvil/releases/latest)

A custom implementation of a Pages server, which allows Git repositories to be used as the source for a static website.

## Why this project

I make use of Forgejo to host some random projects in a homelab environment, and I wanted to be able to host some websites out of it similar to Github and Gitlab Pages.

A pages server for Forgejo already exists, Codeberg's [pages-server](https://codeberg.org/Codeberg/pages-server), but I was having some trouble with its TLS management, and instead
wanted to build my own that simplified the implementation - no TLS support within the server itself, so it can be managed upstream with a system dedicated to the task, like Caddy.

Additionally, I was also interested in building a project like this myself. Please don't use this project in a high-scale production environment without performing a high-degree of
load testing, I have not done this myself.

## Supported source providers

Only one source provider is supported presently.

* [**Forgejo**](https://forgejo.org/) (and Gitea)

## Features

* Stateless, self-contained system
* Serve static HTML, other media, directly from a repository
    * Including private repositories, depending on admin configuration
* Domains provided by the admin (`[repo.]user.pagesdomain`)
* Custom domain support
    * Defined with CNAME or TXT records, pointing to the provided domain
    * **NOTE**: TLS is not directly supported by this project, must be managed by the user with a reverse proxy, or by the admin with another project like Caddy

## End-user usage

Pages is enabled for a repository depending on its name, or by the existence of a special branch within the repository.

Static HTML content is served from these locations directly from the relevant branches, as if it was a folder on a webserver. End users may make use of static site generators by
publishing the output to the relevant branch.

Index documents (index.html) are returned when a file name and extension is not supplied in the path. Note that the Github Pages index rewrite system (`/thing` -> `/thing.html`) is
not supported.

If this server is making use of an API token that has permission to see private repositories for any user or org, then end users are able to make their pages repositories private
while still serving content publicly. If the API token does not have this permission, then the repositories must be public.

Another method of enforcing access may be through registering a well-known service account to generate the API token for this server - the end users would be required to add the
service account as a collaborator to their private repositories before they could be served publicly.

### User pages

A user page is accessible via the domain format `user.pagesdomain`, where `pagesdomain` is the configured domain provided by the admin.

User pages point to repositories within the scope of a user (or org) with the name `pages`, serving static content from the **default branch**.

If a repository with this name does not exist, requests to the domain will result in a `404 NOT FOUND`.

### Repo pages

A repo page is accessible via the domain format `repo.user.pagesdomain`, where `pagesdomain` is the configured domain provided by the admin.

Repo pages point to any repository which has a `pages` branch, serving static content from the **`pages` branch**.

If a repository with the specified name does not exist, or it does not have a `pages` branch, requests to the domain will result in a `404 NOT FOUND`.

### Custom domains

For both user and repo pages, custom domains may be configured (max 128 per repo, arbitrary).

A `.domains` file must be created within the **default branch** of the repository it applies to. Each line in the file represents an allowed domain name for this repository.
Wildcards are not supported. Requests with hostnames which are not present in this file will result in a `404 NOT FOUND`.

A connection to the pages server must then be provided. This may be an A, AAAA, or CNAME record with the name server provider. For a CNAME record, this should point to the provided
domain name; `[repo.]user.pagesdomain`. For A/AAAA records, this would be an IP address that the admin must provide.

From there either the value of the CNAME record, **or a separate TXT record** if using A/AAAA (or reverse proxying, such as with Cloudflare), will be used to link the custom domain
to a repository - checking the custom domain against the `.domains` file.

TLS is not provided out of the box - the admin must configure a separate on-demand TLS system like Caddy, or this must be managed by the end-user with a reverse proxy with TLS
termination capabilities such as Cloudflare etc.
