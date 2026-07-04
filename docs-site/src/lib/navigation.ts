export interface NavItem {
  title: string;
  slug: string;
}

export interface NavSection {
  title: string;
  items: NavItem[];
}

export const navigation: NavSection[] = [
  {
    title: 'Prologue',
    items: [{ title: 'Introduction', slug: 'introduction' }],
  },
  {
    title: 'Getting Started',
    items: [
      { title: 'Installation', slug: 'getting-started/installation' },
      { title: 'Configuration', slug: 'getting-started/configuration' },
      { title: 'Running the App', slug: 'getting-started/running-the-app' },
      { title: 'Project Structure', slug: 'getting-started/project-structure' },
    ],
  },
  {
    title: 'Architecture',
    items: [
      { title: 'Overview', slug: 'architecture/overview' },
      { title: 'Clean Architecture & CQRS', slug: 'architecture/clean-architecture' },
      { title: 'MediatR Pipeline', slug: 'architecture/mediatr-pipeline' },
      { title: 'Extending the Template', slug: 'architecture/extending-the-template' },
    ],
  },
  {
    title: 'Authentication',
    items: [
      { title: 'Overview', slug: 'authentication/overview' },
      { title: 'Login', slug: 'authentication/login' },
      { title: 'Social Login', slug: 'authentication/social-login' },
      { title: 'Refresh Tokens', slug: 'authentication/refresh-tokens' },
      { title: 'Sessions', slug: 'authentication/sessions' },
      { title: 'Logout', slug: 'authentication/logout' },
      { title: 'Email Verification & 2FA', slug: 'authentication/account-security' },
    ],
  },
  {
    title: 'Authorization',
    items: [
      { title: 'RBAC Overview', slug: 'authorization/overview' },
      { title: 'Permissions', slug: 'authorization/permissions' },
      { title: 'Roles', slug: 'authorization/roles' },
      { title: 'HasPermission', slug: 'authorization/has-permission' },
      { title: 'HasRole', slug: 'authorization/has-role' },
    ],
  },
  {
    title: 'Users',
    items: [
      { title: 'Overview', slug: 'users/overview' },
      { title: 'Create User', slug: 'users/create-user' },
      { title: 'Search Users', slug: 'users/search-users' },
      { title: 'Get User', slug: 'users/get-user' },
      { title: 'Assign Roles', slug: 'users/assign-roles' },
    ],
  },
  {
    title: 'Roles API',
    items: [
      { title: 'Overview', slug: 'roles/overview' },
      { title: 'Create Role', slug: 'roles/create-role' },
      { title: 'Search Roles', slug: 'roles/search-roles' },
    ],
  },
  {
    title: 'API Conventions',
    items: [
      { title: 'Response Envelope', slug: 'api/response-envelope' },
      { title: 'API Versioning', slug: 'api/versioning' },
      { title: 'Pagination', slug: 'api/pagination' },
      { title: 'Validation', slug: 'api/validation' },
      { title: 'Exception Handling', slug: 'api/exception-handling' },
      { title: 'Rate Limiting', slug: 'api/rate-limiting' },
      { title: 'Correlation ID', slug: 'api/correlation-id' },
      { title: 'Security Headers', slug: 'api/security-headers' },
      { title: 'Idempotency', slug: 'api/idempotency' },
    ],
  },
  {
    title: 'Configuration',
    items: [
      { title: 'Overview', slug: 'configuration/overview' },
      { title: 'App', slug: 'configuration/app' },
      { title: 'Database', slug: 'configuration/database' },
      { title: 'Cache', slug: 'configuration/cache' },
      { title: 'Mail', slug: 'configuration/mail' },
      { title: 'JWT', slug: 'configuration/jwt' },
      { title: 'Auth', slug: 'configuration/auth' },
      { title: 'Queue', slug: 'configuration/queue' },
      { title: 'CORS', slug: 'configuration/cors' },
      { title: 'Storage', slug: 'configuration/storage' },
      { title: 'Feature Flags', slug: 'configuration/features' },
      { title: 'Socialite', slug: 'configuration/socialite' },
      { title: 'Encryption', slug: 'configuration/encryption' },
      { title: 'Idempotency', slug: 'configuration/idempotency' },
    ],
  },
  {
    title: 'Localization',
    items: [
      { title: 'Overview', slug: 'localization/overview' },
      { title: 'Adding Translations', slug: 'localization/adding-translations' },
    ],
  },
  {
    title: 'Mail',
    items: [
      { title: 'Overview', slug: 'mail/overview' },
      { title: 'Sending Email', slug: 'mail/sending-email' },
    ],
  },
  {
    title: 'Queue & Jobs',
    items: [
      { title: 'Overview', slug: 'queue/overview' },
      { title: 'Creating Jobs', slug: 'queue/creating-jobs' },
    ],
  },
  {
    title: 'Real-time',
    items: [{ title: 'WebSockets', slug: 'realtime/overview' }],
  },
  {
    title: 'Caching',
    items: [
      { title: 'Overview', slug: 'caching/overview' },
      { title: 'Cache Service', slug: 'caching/cache-service' },
    ],
  },
  {
    title: 'Storage',
    items: [{ title: 'Overview', slug: 'storage/overview' }],
  },
  {
    title: 'Feature Flags',
    items: [{ title: 'Overview', slug: 'features/overview' }],
  },
  {
    title: 'Security',
    items: [
      { title: 'Encryption', slug: 'security/encryption' },
      { title: 'Hashing', slug: 'security/hashing' },
    ],
  },
  {
    title: 'Documents',
    items: [
      { title: 'Excel', slug: 'documents/excel' },
      { title: 'PDF', slug: 'documents/pdf' },
    ],
  },
  {
    title: 'Utilities',
    items: [
      { title: 'Clock', slug: 'utilities/clock' },
      { title: 'Str Helpers', slug: 'utilities/str' },
      { title: 'Helpers & Extensions', slug: 'utilities/helpers' },
    ],
  },
  {
    title: 'Database',
    items: [
      { title: 'Overview', slug: 'database/overview' },
      { title: 'Migrations', slug: 'database/migrations' },
      { title: 'Seeders', slug: 'database/seeders' },
    ],
  },
  {
    title: 'Tools',
    items: [
      { title: 'do CLI', slug: 'tools/do-cli' },
      { title: 'dotnet new Template', slug: 'tools/dotnet-template' },
    ],
  },
  {
    title: 'Testing',
    items: [{ title: 'Overview', slug: 'testing/overview' }],
  },
  {
    title: 'Aspire',
    items: [{ title: 'Overview', slug: 'aspire/overview' }],
  },
  {
    title: 'Deployment',
    items: [{ title: 'Docker', slug: 'deployment/docker' }],
  },
];

export function getNavItemBySlug(slug: string): NavItem | undefined {
  for (const section of navigation) {
    const item = section.items.find((i) => i.slug === slug);
    if (item) return item;
  }
  return undefined;
}

export function getAllNavItems(): NavItem[] {
  return navigation.flatMap((section) => section.items);
}

export function getAdjacentSlugs(slug: string): { prev?: NavItem; next?: NavItem } {
  const items = getAllNavItems();
  const index = items.findIndex((i) => i.slug === slug);
  if (index === -1) return {};
  return {
    prev: index > 0 ? items[index - 1] : undefined,
    next: index < items.length - 1 ? items[index + 1] : undefined,
  };
}
