'use strict';

/**
 * Auth service configuration - all settings sourced from environment variables.
 * Defaults are safe for local Docker Compose development.
 */
const config = {
  port: parseInt(process.env.AUTH_PORT || '3000', 10),

  // PostgreSQL
  db: {
    host: process.env.DB_HOST || 'postgres',
    port: parseInt(process.env.DB_PORT || '5432', 10),
    database: process.env.DB_NAME || 'iron_exiles_auth',
    user: process.env.DB_USER || 'auth_service',
    password: process.env.DB_PASSWORD || 'dev_password',
    max: parseInt(process.env.DB_POOL_MAX || '10', 10),
  },

  // Redis (rate limiting)
  redis: {
    host: process.env.REDIS_HOST || 'redis',
    port: parseInt(process.env.REDIS_PORT || '6379', 10),
  },

  // JWT
  jwt: {
    secret: process.env.JWT_SECRET || 'dev-secret-change-in-production',
    expiresIn: process.env.JWT_EXPIRES_IN || '24h',
    issuer: 'iron-exiles-auth',
  },

  // Rate limiting (BR-3: 5 attempts / minute / IP)
  rateLimit: {
    windowMs: parseInt(process.env.RATE_LIMIT_WINDOW_MS || '60000', 10),
    maxAttempts: parseInt(process.env.RATE_LIMIT_MAX || '5', 10),
  },

  // Password policy (BR-1)
  password: {
    minLength: 8,
    bcryptRounds: parseInt(process.env.BCRYPT_ROUNDS || '12', 10),
  },

  // Auth mode flag (BR-4: bypass for dev)
  authMode: {
    enabled: process.env.AUTH_MODE_ENABLED !== 'false',
  },
};

module.exports = config;
