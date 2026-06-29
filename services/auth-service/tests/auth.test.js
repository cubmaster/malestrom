'use strict';

const { describe, it, before, after, beforeEach } = require('node:test');
const assert = require('node:assert/strict');
const request = require('supertest');

// Set test environment before importing app
process.env.DB_HOST = process.env.DB_HOST || 'localhost';
process.env.DB_NAME = process.env.DB_NAME || 'iron_exiles_auth_test';
process.env.JWT_SECRET = 'test-secret-key';
process.env.BCRYPT_ROUNDS = '4'; // Fast for tests
process.env.RATE_LIMIT_MAX = '100'; // High limit for tests

const app = require('../src/index');
const pool = require('../src/db/pool');

describe('Auth Service', () => {
  before(async () => {
    // Run migration
    const fs = require('fs');
    const path = require('path');
    const migrationSql = fs.readFileSync(
      path.join(__dirname, '../migrations/001_create_accounts.sql'),
      'utf8'
    );
    await pool.query(migrationSql);
  });

  beforeEach(async () => {
    // Clean accounts table between tests
    await pool.query('DELETE FROM accounts');
  });

  after(async () => {
    await pool.end();
  });

  describe('GET /health', () => {
    it('returns healthy status', async () => {
      const res = await request(app).get('/health');
      assert.equal(res.status, 200);
      assert.equal(res.body.status, 'healthy');
      assert.equal(res.body.service, 'auth-service');
    });
  });

  describe('POST /register', () => {
    it('creates account with valid email and password', async () => {
      const res = await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'SecurePass123' });

      assert.equal(res.status, 201);
      assert.ok(res.body.account_id);
      assert.equal(res.body.email, 'player@test.com');
      assert.ok(res.body.created_at);
    });

    it('rejects missing email', async () => {
      const res = await request(app)
        .post('/register')
        .send({ password: 'SecurePass123' });

      assert.equal(res.status, 400);
      assert.equal(res.body.error, 'validation_error');
    });

    it('rejects missing password', async () => {
      const res = await request(app)
        .post('/register')
        .send({ email: 'player@test.com' });

      assert.equal(res.status, 400);
      assert.equal(res.body.error, 'validation_error');
    });

    it('rejects invalid email format', async () => {
      const res = await request(app)
        .post('/register')
        .send({ email: 'not-an-email', password: 'SecurePass123' });

      assert.equal(res.status, 400);
      assert.equal(res.body.error, 'validation_error');
    });

    it('rejects password shorter than 8 characters (BR-1)', async () => {
      const res = await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'short' });

      assert.equal(res.status, 400);
      assert.match(res.body.message, /at least 8 characters/);
    });

    it('rejects duplicate email', async () => {
      await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'SecurePass123' });

      const res = await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'DifferentPass1' });

      assert.equal(res.status, 409);
      assert.equal(res.body.error, 'conflict');
    });

    it('normalizes email to lowercase', async () => {
      const res = await request(app)
        .post('/register')
        .send({ email: 'Player@Test.COM', password: 'SecurePass123' });

      assert.equal(res.status, 201);
      assert.equal(res.body.email, 'player@test.com');
    });
  });

  describe('POST /login', () => {
    beforeEach(async () => {
      await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'SecurePass123' });
    });

    it('returns JWT for valid credentials', async () => {
      const res = await request(app)
        .post('/login')
        .send({ email: 'player@test.com', password: 'SecurePass123' });

      assert.equal(res.status, 200);
      assert.ok(res.body.token);
      assert.ok(res.body.account_id);
      assert.ok(res.body.expires_at);
    });

    it('rejects invalid password', async () => {
      const res = await request(app)
        .post('/login')
        .send({ email: 'player@test.com', password: 'WrongPassword' });

      assert.equal(res.status, 401);
      assert.equal(res.body.error, 'login_failed');
    });

    it('rejects non-existent email', async () => {
      const res = await request(app)
        .post('/login')
        .send({ email: 'nobody@test.com', password: 'SecurePass123' });

      assert.equal(res.status, 401);
      assert.equal(res.body.error, 'login_failed');
    });

    it('rejects missing credentials', async () => {
      const res = await request(app)
        .post('/login')
        .send({});

      assert.equal(res.status, 400);
    });
  });

  describe('POST /validate', () => {
    it('validates a valid token', async () => {
      // Register and login to get a token
      await request(app)
        .post('/register')
        .send({ email: 'player@test.com', password: 'SecurePass123' });

      const loginRes = await request(app)
        .post('/login')
        .send({ email: 'player@test.com', password: 'SecurePass123' });

      const res = await request(app)
        .post('/validate')
        .send({ token: loginRes.body.token });

      assert.equal(res.status, 200);
      assert.equal(res.body.valid, true);
      assert.ok(res.body.account_id);
      assert.equal(res.body.email, 'player@test.com');
    });

    it('rejects an invalid token', async () => {
      const res = await request(app)
        .post('/validate')
        .send({ token: 'invalid.token.here' });

      assert.equal(res.status, 401);
      assert.equal(res.body.valid, false);
    });

    it('rejects missing token', async () => {
      const res = await request(app)
        .post('/validate')
        .send({});

      assert.equal(res.status, 400);
    });
  });
});
