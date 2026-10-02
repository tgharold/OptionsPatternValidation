# Security Policy

## Supported versions

Security fixes go into the latest release only. If you use an older version, upgrade to the latest release first.

| Version | Supported |
|---------|-----------|
| Latest release on [nuget.org](https://www.nuget.org/packages/OptionsPatternValidation) | Yes |
| Older releases | No |

## Report a vulnerability

Do not open a public issue for a security problem.

Use GitHub's private reporting instead:

1. Open the [Security tab](https://github.com/tgharold/OptionsPatternValidation/security) of this repository.
2. Select **Report a vulnerability**.
3. Describe the problem and include a minimal options class and configuration that reproduce it.

Please include:

- The package version and target framework (for example, `net8.0`).
- What you expected to happen and what happened instead.
- The impact, if you know it.

This is a one-person project. I aim to reply within 30 days. I will tell you whether I accept the report and agree on a disclosure date with you. I will credit you in the release notes unless you ask me not to.

## What counts as a vulnerability

This library binds configuration sections to options classes and registers validation for them. The configuration usually comes from the operator, but some sources can be influenced by an attacker. These are in scope:

- **Validation bypass.** An invalid options object is accepted when validation should reject it. An example is a registration method that skips the validation it promises.
- **Denial of service.** Crafted configuration crashes the process or uses unbounded time or memory.

These are not in scope:

- Bugs that have no security impact. Open a normal [issue](https://github.com/tgharold/OptionsPatternValidation/issues) for those.
- Vulnerabilities in your own validation attributes, validators or configuration sources, or in .NET itself. Report those to the code's owner.
- Exceptions shown to end users. This is backend code. The calling application must catch exceptions and decide what to display.
- Vulnerabilities in dependencies. Dependabot tracks those for this repository. Report problems in the recursive object walker to [RecursiveDataAnnotationsValidation](https://github.com/tgharold/RecursiveDataAnnotationsValidation/security).

## Disclosure

After a fix ships, I publish a [GitHub security advisory](https://github.com/tgharold/OptionsPatternValidation/security/advisories) and add an entry to the [changelog](CHANGELOG.md).
