'use strict';

const { Router } = require('express');
const bcrypt = require('bcrypt');
const jwt = require('jsonwebtoken');
const pool = require('../db/pool');
const config = require('../config');

const router = Router();

/**
 * POST /register
 * Creates a new account with email + password.
 * Returns account_id on success.
 */
router.post('/register', async (req, res) => {
  try {
    const { email, password } = req.body;

    // Validate input
    if (!email || !password) {
      return res.status(400).json({
        error: 'validation_error',
        message: 'Email and password are required.',
      });
    }

    // Email format check
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      return res.status(400).json({
        error: 'validation_error',
        message: 'Invalid email format.',
      });
    }

    // BR-1: Minimum password length 8
    if (password.length < config.password.minLength) {
      return res.status(400).json({
        error: 'validation_error',
        message: `Password must be at least ${config.password.minLength} characters.`,
      });
    }

    // BR-1: Hash password (never stored plaintext)
    const passwordHash = await bcrypt.hash(password, config.password.bcryptRounds);

    // Insert account
    const result = await pool.query(
      `INSERT INTO accounts (email, password_hash)
       VALUES ($1, $2)
       RETURNING account_id, email, created_at`,
      [email.toLowerCase().trim(), passwordHash]
    );

    const account = result.rows[0];

    console.log(`[AuthService] account_registered: ${account.account_id} (${account.email})`);

    return res.status(201).json({
      account_id: account.account_id,
      email: account.email,
      created_at: account.created_at,
    });
  } catch (err) {
    // Duplicate email
    if (err.code === '23505') {
      return res.status(409).json({
        error: 'conflict',
        message: 'An account with this email already exists.',
      });
    }

    console.error('[AuthService] Register error:', err.message);
    return res.status(500).json({
      error: 'internal_error',
      message: 'Registration failed. Please try again.',
    });
  }
});

/**
 * POST /login
 * Authenticates with email + password, returns JWT.
 */
router.post('/login', async (req, res) => {
  try {
    const { email, password } = req.body;

    if (!email || !password) {
      return res.status(400).json({
        error: 'validation_error',
        message: 'Email and password are required.',
      });
    }

    // Look up account
    const result = await pool.query(
      'SELECT account_id, email, password_hash FROM accounts WHERE email = $1',
      [email.toLowerCase().trim()]
    );

    if (result.rows.length === 0) {
      // Timing-safe: still hash to prevent timing attacks
      await bcrypt.hash('dummy', 10);
      return res.status(401).json({
        error: 'login_failed',
        message: 'Invalid email or password.',
      });
    }

    const account = result.rows[0];

    // Verify password
    const valid = await bcrypt.compare(password, account.password_hash);
    if (!valid) {
      console.log(`[AuthService] login_failed: invalid password for ${account.email}`);
      return res.status(401).json({
        error: 'login_failed',
        message: 'Invalid email or password.',
      });
    }

    // BR-2: Issue JWT (expires within 24h, configurable)
    const token = jwt.sign(
      {
        sub: account.account_id,
        email: account.email,
      },
      config.jwt.secret,
      {
        expiresIn: config.jwt.expiresIn,
        issuer: config.jwt.issuer,
      }
    );

    // Calculate expiry for client
    const decoded = jwt.decode(token);
    const expiresAt = new Date(decoded.exp * 1000).toISOString();

    console.log(`[AuthService] login_success: ${account.account_id}`);

    return res.status(200).json({
      token,
      account_id: account.account_id,
      expires_at: expiresAt,
    });
  } catch (err) {
    console.error('[AuthService] Login error:', err.message);
    return res.status(500).json({
      error: 'internal_error',
      message: 'Login failed. Please try again.',
    });
  }
});

/**
 * POST /validate
 * Validates a JWT token. Used by game server to verify client tokens.
 * Returns decoded payload if valid.
 */
router.post('/validate', async (req, res) => {
  try {
    const { token } = req.body;

    if (!token) {
      return res.status(400).json({
        error: 'validation_error',
        message: 'Token is required.',
      });
    }

    const decoded = jwt.verify(token, config.jwt.secret, {
      issuer: config.jwt.issuer,
    });

    return res.status(200).json({
      valid: true,
      account_id: decoded.sub,
      email: decoded.email,
      expires_at: new Date(decoded.exp * 1000).toISOString(),
    });
  } catch (err) {
    if (err.name === 'TokenExpiredError') {
      return res.status(401).json({
        valid: false,
        error: 'token_expired',
        message: 'Token has expired.',
      });
    }

    return res.status(401).json({
      valid: false,
      error: 'token_invalid',
      message: 'Token is invalid.',
    });
  }
});

module.exports = router;
