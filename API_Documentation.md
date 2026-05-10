# Ecommerce API Documentation

## Overview

This document describes all available REST API endpoints for the ecommerce system. The API is organized around standard RESTful conventions, returns JSON responses, and uses JWT Bearer tokens for authentication.

**Base URL:** `https://ecommerceproject-webapi.fly.dev/swagger`

---

## Authentication & Authorization

The API uses **JWT Bearer token** authentication. Protected endpoints require an `Authorization` header:

```
Authorization: Bearer <token>
```

### Roles & Policies

| Role / Policy | Description |
|---|---|
| `AdminOnly` | Requests must be authenticated with an Admin role |
| `CustomerOnly` | Requests must be authenticated with a Customer role |
| `Customer, Admin` | Either role may access the endpoint |
| `[AllowAnonymous]` | No authentication required |

---

## Controllers

1. [Auth](#1-auth)
2. [Admin](#2-admin)
3. [Customer](#3-customer)
4. [Product](#4-product)
5. [Category](#5-category)
6. [Cart](#6-cart)
7. [Order](#7-order)
8. [Review](#8-review)

---

## 1. Auth

**Base Route:** `POST /api/v1/auth`

Handles public-facing authentication — customer registration and login. Both endpoints are publicly accessible and return a JWT token on success.

---

### POST `/api/v1/auth/login`

**Description:** Authenticates a user (customer or admin) using their email and password. Returns a JWT token and basic user information to be used in subsequent authenticated requests.

**Auth:** Public (No token required)

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "YourPassword123!"
}
```

**Workflow:**
1. Client submits email and password.
2. Service validates credentials against the database.
3. On success, a signed JWT token is generated and returned.
4. Client stores the token and attaches it to future requests via the `Authorization` header.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Login successful. Returns JWT token and user info. |
| `400 Bad Request` | Missing or malformed request body. |
| `401 Unauthorized` | Invalid email or password. |

**Response Example (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5...",
  "expiresAt": "2025-06-01T12:00:00Z",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "user@example.com",
    "role": "Customer"
  }
}
```

---

### POST `/api/v1/auth/register`

**Description:** Registers a new customer account. On success, the customer is created in the system and a JWT token is returned so the user is immediately logged in without a separate login step.

**Auth:** Public (No token required)

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane@example.com",
  "password": "SecurePass123!",
  "phoneNumber": "+2348012345678"
}
```

**Workflow:**
1. Client submits registration details.
2. Service validates the data (unique email, password strength, etc.).
3. A new customer record is created and assigned the `Customer` role.
4. A JWT token is generated and returned alongside the new user's info.

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Customer registered successfully. Returns token and user info. |
| `400 Bad Request` | Validation failed (e.g. email already taken, weak password). |

---

## 2. Admin

**Base Route:** `GET/POST /api/v1/admin`

**Auth Required:** All endpoints require an authenticated Admin (`AdminOnly` policy).

Manages admin accounts — creation and lookup. Admins cannot be deleted through this API; deletion would be handled at the infrastructure level.

---

### POST `/api/v1/admin`

**Description:** Creates a new admin account. Only existing admins can create new admins — this prevents unauthorized escalation of privileges.

**Auth:** Admin only

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Smith",
  "email": "admin@example.com",
  "password": "AdminPass123!"
}
```

**Workflow:**
1. Authenticated admin submits new admin details.
2. Service creates the account and assigns the `Admin` role.
3. Returns the newly created admin record.

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Admin created successfully. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token provided. |
| `403 Forbidden` | Authenticated user is not an Admin. |

---

### GET `/api/v1/admin`

**Description:** Returns a list of all admin accounts in the system.

**Auth:** Admin only

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns array of admin objects. |

---

### GET `/api/v1/admin/{id}`

**Description:** Retrieves a specific admin's profile by their GUID.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Admin's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Admin found and returned. |
| `404 Not Found` | No admin exists with the given ID. |

---

### GET `/api/v1/admin/{email}`

**Description:** Retrieves a specific admin's profile by their email address.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `email` | `string` | Admin's email address |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Admin found and returned. |
| `404 Not Found` | No admin exists with the given email. |

---

### GET `/api/v1/admin/reference/{reference}`

**Description:** Retrieves a specific admin by their unique reference code (system-generated identifier).

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `reference` | `string` | Admin's reference code |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Admin found and returned. |
| `404 Not Found` | No admin exists with the given reference code. |

---

## 3. Customer

**Base Route:** `/api/customer`

**Auth Required:** All endpoints require authentication. Role requirements vary per endpoint.

Manages customer accounts. Admins have full access to all customer records. Customers can only view and edit their own profile.

---

### GET `/api/customer`

**Description:** Returns a paginated, searchable list of all customers. Supports filtering by name, email, or reference number.

**Auth:** Admin only

**Query Parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `searchTerm` | `string` | `null` | Filter by name, email, or ref number |
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page (max 100) |
| `sortBy` | `string` | `null` | Sort field e.g. `firstname`, `createddate_desc` |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns paginated list of customers. |

---

### GET `/api/customer/{id}`

**Description:** Returns a specific customer's full profile by their GUID.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Customer's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Customer found and returned. |
| `404 Not Found` | No customer with the given ID exists. |

---

### GET `/api/customer/me`

**Description:** Returns the currently logged-in customer's own profile. The customer ID is extracted from the JWT token claims — no ID needs to be passed in the URL.

**Auth:** Customer only

**Workflow:**
1. Token is validated and the `userId` claim is extracted.
2. Service fetches and returns that customer's profile.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns the logged-in customer's profile. |
| `401 Unauthorized` | No valid token or `userId` claim missing. |
| `404 Not Found` | Customer record not found. |

---

### GET `/api/customer/reference_number/{refNo}`

**Description:** Fetches a customer by their unique reference number.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `refNo` | `string` | Customer's reference number |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Customer found and returned. |
| `404 Not Found` | No customer with the given reference number. |

---

### GET `/api/customer/email/{email}`

**Description:** Fetches a customer by their email address.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `email` | `string` | Customer's email address |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Customer found and returned. |
| `404 Not Found` | No customer with the given email. |

---

### POST `/api/customer`

**Description:** Creates a new customer account directly (admin-created, as opposed to self-registration via `/auth/register`). Useful for manually onboarding customers.

**Auth:** Admin only

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Doe",
  "email": "jane@example.com",
  "password": "SecurePass123!",
  "phoneNumber": "+2348012345678"
}
```

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Customer created successfully. |
| `400 Bad Request` | Validation failed. |

---

### PUT `/api/customer/me`

**Description:** Allows the currently logged-in customer to update their own profile (name, phone, profile picture, etc.). Uses `multipart/form-data` to support file uploads (e.g. profile image).

**Auth:** Customer only

**Request:** `multipart/form-data`

**Workflow:**
1. Token is validated and `userId` is extracted from claims.
2. Request body is mapped and the customer's record is updated.
3. Returns `204 No Content` on success — no body is returned.

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Profile updated successfully. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | Customer profile not found. |

---

### DELETE `/api/customer/{id}`

**Description:** Soft-deletes a customer record (marks as inactive — does not permanently remove data). Used by admins to deactivate customer accounts.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Customer's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Customer soft-deleted successfully. |
| `404 Not Found` | No customer with the given ID exists. |

---

## 4. Product

**Base Route:** `/api/product`

Read endpoints are publicly accessible. Write operations (create, update, delete) require Admin authentication.

---

### GET `/api/product`

**Description:** Returns a paginated, searchable, filterable list of products. Supports category filtering and sorting.

**Auth:** Public

**Query Parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `searchTerm` | `string` | `null` | Search by product name or description |
| `categoryId` | `guid` | `null` | Filter by category |
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page |
| `sortBy` | `string` | `null` | Sort field (e.g. `price_desc`, `name`) |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns paginated list of products. |

---

### GET `/api/product/{id}`

**Description:** Returns the full details of a single product by its GUID.

**Auth:** Public

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Product's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Product found and returned. |
| `404 Not Found` | No product with the given ID exists. |

---

### POST `/api/product`

**Description:** Creates a new product. Uses `multipart/form-data` to support image uploads alongside product data.

**Auth:** Admin only

**Request:** `multipart/form-data`

```
name: "Wireless Headphones"
description: "Noise cancelling over-ear headphones"
price: 49999
stock: 100
categoryId: "3fa85f64-5717-4562-b3fc-2c963f66afa6"
image: <file>
```

**Workflow:**
1. Admin submits product details and optional image.
2. Image is uploaded and stored; URL is saved to the product record.
3. Product is saved and returned with its generated ID.

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Product created successfully. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |

---

### PUT `/api/product/{id}`

**Description:** Updates an existing product's details and/or image.

**Auth:** Admin only

**Request:** `multipart/form-data`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Product's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Product updated and returned. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |
| `404 Not Found` | No product with the given ID exists. |

---

### DELETE `/api/product/{id}`

**Description:** Deletes a product from the system. This may be a soft or hard delete depending on the service implementation.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Product's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Product deleted successfully. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |
| `404 Not Found` | No product with the given ID exists. |

---

## 5. Category

**Base Route:** `/api/category`

Read endpoints are public. Write operations require Admin authentication.

---

### GET `/api/category`

**Description:** Returns all product categories. No pagination — intended for populating dropdowns or filter menus.

**Auth:** Public

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns full list of categories. |

---

### GET `/api/category/{id}`

**Description:** Returns details of a single category by its GUID.

**Auth:** Public

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Category's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Category found and returned. |
| `404 Not Found` | No category with the given ID exists. |

---

### POST `/api/category`

**Description:** Creates a new product category.

**Auth:** Admin only

**Request Body:**
```json
{
  "name": "Electronics",
  "description": "Electronic devices and accessories"
}
```

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Category created successfully. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |

---

### PUT `/api/category/{id}`

**Description:** Updates an existing category's name or description.

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Category's unique identifier |

**Request Body:**
```json
{
  "name": "Electronics & Gadgets",
  "description": "Updated description"
}
```

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Category updated and returned. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |
| `404 Not Found` | No category with the given ID exists. |

---

### DELETE `/api/category/{id}`

**Description:** Deletes a category. Note: if products are linked to this category, the service layer should handle the dependency appropriately (e.g. prevent deletion or nullify the foreign key).

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Category's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Category deleted successfully. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |
| `404 Not Found` | No category with the given ID exists. |

---

## 6. Cart

**Base Route:** `/api/cart`

**Auth Required:** All cart endpoints require a valid token with a `Customer` or `Admin` role. The cart is always scoped to the currently logged-in user — no user ID is passed explicitly.

---

### GET `/api/cart`

**Description:** Returns the current user's active cart, including all items, quantities, and computed totals.

**Auth:** Customer or Admin

**Workflow:**
1. `userId` is extracted from the JWT token.
2. The cart belonging to that user is fetched and returned.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns the user's cart. |
| `401 Unauthorized` | No valid token. |

---

### POST `/api/cart/items`

**Description:** Adds a product to the current user's cart. If the product already exists in the cart, the quantity is incremented.

**Auth:** Customer or Admin

**Request Body:**
```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "quantity": 2
}
```

**Workflow:**
1. `userId` is resolved from the token.
2. The product ID and quantity are validated (product must exist and be in stock).
3. The item is added or the quantity is updated if already present.
4. Returns the updated cart.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Item added. Returns updated cart. |
| `400 Bad Request` | Invalid product ID or quantity. |
| `401 Unauthorized` | No valid token. |

---

### PUT `/api/cart/items/{cartItemId}`

**Description:** Updates the quantity of a specific item already in the cart.

**Auth:** Customer or Admin

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `cartItemId` | `guid` | The cart item's unique identifier |

**Request Body:**
```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "quantity": 5
}
```

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Item updated. Returns updated cart. |
| `400 Bad Request` | Invalid quantity. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | Cart item not found. |

---

### DELETE `/api/cart/items/{cartItemId}`

**Description:** Removes a specific item from the cart entirely.

**Auth:** Customer or Admin

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `cartItemId` | `guid` | The cart item's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Item removed. Returns updated cart. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | Cart item not found. |

---

### DELETE `/api/cart`

**Description:** Clears all items from the current user's cart. Useful for after checkout or when a customer wants to start fresh.

**Auth:** Customer or Admin

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Cart cleared successfully. |
| `401 Unauthorized` | No valid token. |

---

### POST `/api/cart/merge`

**Description:** Merges a guest/anonymous cart into the logged-in customer's saved cart. Used when a customer adds items before logging in and then authenticates — their guest cart items are preserved.

**Auth:** Customer or Admin

**Request Body:**
```json
{
  "guestCartId": "guest-session-uuid-or-token"
}
```

**Workflow:**
1. The guest cart is identified by the provided session ID or token.
2. Items from the guest cart are merged into the authenticated user's cart.
3. Conflicts (duplicate products) are resolved by summing quantities.
4. Returns the merged cart.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Carts merged. Returns the merged cart. |
| `400 Bad Request` | Invalid or missing guest cart reference. |
| `401 Unauthorized` | No valid token. |

---

## 7. Order

**Base Route:** `/api/order`

Handles the full order lifecycle — from checkout and payment initiation to order tracking and status management. Payment is processed via **Paystack**.

---

### POST `/api/order/checkout`

**Description:** Initiates a checkout from the customer's active cart. Creates an order record and returns a Paystack payment authorization URL for the customer to complete payment.

**Auth:** Customer or Admin

**Request Body:**
```json
{
  "deliveryAddress": "12 Main Street, Lagos, Nigeria",
  "notes": "Please leave at the door"
}
```

**Workflow:**
1. The customer's active cart is fetched using their token identity.
2. An order is created with status `Pending` and items copied from the cart.
3. A Paystack payment link is generated for the order total.
4. The cart may be cleared or held pending payment confirmation.
5. The payment URL is returned to the client.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns Paystack payment authorization URL and order details. |
| `400 Bad Request` | Cart is empty or validation failed. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Insufficient role. |

**Response Example (200):**
```json
{
  "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "orderNumber": "ORD-20250501-0042",
  "paymentUrl": "https://checkout.paystack.com/xyz123",
  "totalAmount": 49999,
  "status": "Pending"
}
```

---

### GET `/api/order/my-orders`

**Description:** Returns a paginated list of the currently logged-in customer's orders. Scoped to the authenticated user's identity.

**Auth:** Customer or Admin

**Query Parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns paginated list of the user's orders. |
| `401 Unauthorized` | No valid token. |

---

### GET `/api/order/{id}`

**Description:** Returns a specific order by its GUID. Customers may only access their own orders; admins can access any order. Authorization enforcement is handled at the service layer.

**Auth:** Any authenticated user

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Order's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Order found and returned. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | Order not found or not accessible to the requesting user. |

---

### GET `/api/order/number/{orderNumber}`

**Description:** Fetches an order using its human-readable order number (e.g. `ORD-20250501-0042`). Useful for customer-facing order tracking pages.

**Auth:** Any authenticated user

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `orderNumber` | `string` | Human-readable order number |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Order found and returned. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | No order with the given order number. |

---

### GET `/api/order`

**Description:** Returns a paginated list of all orders in the system. Admin use only — for order management dashboards.

**Auth:** Admin only

**Query Parameters:**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | `int` | `1` | Page number |
| `pageSize` | `int` | `10` | Items per page |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns paginated list of all orders. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |

---

### PUT `/api/order/{id}/status`

**Description:** Updates the fulfillment status of an order (e.g. from `Pending` → `Processing` → `Shipped` → `Delivered`).

**Auth:** Admin only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `id` | `guid` | Order's unique identifier |

**Request Body:**
```json
{
  "status": "Shipped"
}
```

**Possible Status Values(strictly ordered):** `Pending`, `Processing`, `Shipped`, `Delivered`, `Cancelled`

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Order status updated and returned. |
| `400 Bad Request` | Invalid status value. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Authenticated user is not an Admin. |
| `404 Not Found` | Order not found. |

---

### POST `/api/order/webhook`

**Description:** Paystack webhook endpoint. Called by Paystack's servers after a payment event (success, failure, etc.). Verifies the request signature and processes the payment result — updating the order status accordingly.

**Auth:** Public (No token — called by Paystack, not the client)

> ⚠️ **Security Note:** This endpoint validates the `x-paystack-signature` header using HMAC-SHA512 with your Paystack secret key. Requests with invalid signatures are silently ignored. Always return `200 OK` to Paystack regardless of whether the event was processed — failure to do so causes Paystack to retry.

**Headers:**

| Header | Description |
|---|---|
| `x-paystack-signature` | HMAC-SHA512 hash of the raw request body |

**Workflow:**
1. Raw request body is read (preserving exact bytes for signature verification).
2. The `x-paystack-signature` header is extracted.
3. The service verifies the signature against your Paystack secret key.
4. If valid, the event type is parsed (e.g. `charge.success`).
5. The relevant order is located by the Paystack reference.
6. Order status is updated (e.g. `Pending` → `Processing`) and the cart is cleared.
7. Returns `200 OK` unconditionally.

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Always returned, whether the event was processed or skipped. |

---

## 8. Review

**Base Route:** `/api/review`

Allows customers to leave, update, and delete reviews on products they have purchased. Public users can read reviews.

---

### GET `/api/review/product/{productId}`

**Description:** Returns all reviews for a given product. Publicly accessible — no login required.

**Auth:** Public

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `productId` | `guid` | The product's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Returns list of reviews for the product. |
| `404 Not Found` | Product not found. |

---

### POST `/api/review`

**Description:** Creates a new review for a product the customer has purchased. The service layer should enforce that a review can only be submitted for a product the user has a completed order for.

**Auth:** Customer or Admin

**Request Body:**
```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "rating": 5,
  "comment": "Excellent product, fast delivery!"
}
```

**Workflow:**
1. Authenticated user submits a review for a product.
2. Service verifies the user has a completed order containing that product.
3. Review is saved and linked to the product and user.
4. Returns the created review with a `201 Created` status, and redirects to the product's review list.

**Responses:**

| Status | Description |
|---|---|
| `201 Created` | Review created successfully. |
| `400 Bad Request` | Validation failed or user has not purchased this product. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | Insufficient role. |

---

### PUT `/api/review/{reviewId}`

**Description:** Updates an existing review. Customers may only update their own reviews; the service layer should enforce ownership.

**Auth:** Customer or Admin

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `reviewId` | `guid` | The review's unique identifier |

**Request Body:**
```json
{
  "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "rating": 4,
  "comment": "Updated my review — still great but delivery was slightly delayed."
}
```

**Responses:**

| Status | Description |
|---|---|
| `200 OK` | Review updated and returned. |
| `400 Bad Request` | Validation failed. |
| `401 Unauthorized` | No valid token. |
| `404 Not Found` | Review not found. |

---

### DELETE `/api/review/{reviewId}`

**Description:** Deletes a review. Customers can delete their own reviews; admins can delete any review (e.g. for moderation).

**Auth:** Any authenticated user (ownership enforced at service layer)

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| `reviewId` | `guid` | The review's unique identifier |

**Responses:**

| Status | Description |
|---|---|
| `204 No Content` | Review deleted successfully. |
| `401 Unauthorized` | No valid token. |
| `403 Forbidden` | User does not own this review and is not an Admin. |
| `404 Not Found` | Review not found. |

---

## Error Format

All error responses follow a consistent JSON shape:

```json
{
  "message": "A human-readable description of the error."
}
```

Validation errors (400) may include field-level detail depending on the model binding behaviour configured in your app.

---

## Typical Workflows

### Customer Purchase Flow

```
1. POST /api/v1/auth/register  →  Create account & receive token
2. GET  /api/product           →  Browse products
3. POST /api/cart/items        →  Add items to cart
4. PUT  /api/cart/items/{id}   →  Adjust quantities
5. POST /api/order/checkout    →  Place order & receive Paystack URL
6. [Customer pays on Paystack]
7. POST /api/order/webhook     →  Paystack notifies your server
8. GET  /api/order/my-orders   →  Customer tracks their orders
9. POST /api/review            →  Customer leaves a review
```

### Admin Management Flow

```
1. POST /api/v1/auth/login        →  Admin logs in
2. GET  /api/customer             →  View all customers
3. GET  /api/order                →  View all orders
4. PUT  /api/order/{id}/status    →  Update order to Shipped/Delivered
5. POST /api/product              →  Add new products
6. DELETE /api/review/{id}        →  Moderate inappropriate reviews
```
