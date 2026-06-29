'use strict';

const express = require('express');
const helmet = require('helmet');
const cors = require('cors');
const config = require('./config');
const authRoutes = require('./routes/auth');
const { createLoginRateLimiter } = require('./middleware/rateLimiter');
const pool = require('./db/pool');

const app = express();

// Security middleware
app.use(helmet());
app.use(cors());
app.use(express.json({ limit: '16kb' }));

// Trust proxy for correct IP in rate limiter (Docker networking)
app.set('trust proxy', 1);

// Health check (no rate limiting)
app.get('/health', async (req, res) => {
  try {
    await pool.query('SELECT 1');
    res.status(200).json({ status: 'healthy', service: 'auth-service' });
  } catch (err) {
    res.status(503).json({ status: 'unhealthy', error: err.message });
  }
});

// Apply rate limiter to login endpoint only (BR-3)
app.use('/login', createLoginRateLimiter());

// Auth routes
app.use('/', authRoutes);

// 404 handler
app.use((req, res) => {
  res.status(404).json({ error: 'not_found', message: 'Endpoint not found.' });
});

// Error handler
app.use((err, req, res, _next) => {
  console.error('[AuthService] Unhandled error:', err.message);
  res.status(500).json({ error: 'internal_error', message: 'An unexpected error occurred.' });
});

// Start server
const server = app.listen(config.port, () => {
  console.log(`[AuthService] Running on port ${config.port}`);
  console.log(`[AuthService] Auth mode: ${config.authMode.enabled ? 'ENABLED' : 'DISABLED (bypass)'}`);
});

// Graceful shutdown
process.on('SIGTERM', async () => {
  console.log('[AuthService] SIGTERM received, shutting down gracefully...');
  server.close(() => {
    pool.end().then(() => process.exit(0));
  });
});

module.exports = app; // Export for testing
