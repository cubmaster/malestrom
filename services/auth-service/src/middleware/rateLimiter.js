'use strict';

const rateLimit = require('express-rate-limit');
const config = require('../config');

/**
 * BR-3: Failed login rate limit: 5 attempts / minute / IP (basic).
 * Uses in-memory store by default; Redis store available for multi-instance.
 */
function createLoginRateLimiter() {
  return rateLimit({
    windowMs: config.rateLimit.windowMs,
    max: config.rateLimit.maxAttempts,
    message: {
      error: 'rate_limited',
      message: `Too many login attempts. Please try again after ${Math.ceil(config.rateLimit.windowMs / 1000)} seconds.`,
    },
    standardHeaders: true,
    legacyHeaders: false,
    // Only count failed attempts (skip successful logins)
    skipSuccessfulRequests: true,
    keyGenerator: (req) => {
      // Rate limit by IP
      return req.ip || req.connection.remoteAddress || 'unknown';
    },
  });
}

module.exports = { createLoginRateLimiter };
