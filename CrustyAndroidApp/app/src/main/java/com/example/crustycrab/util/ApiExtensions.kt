package com.example.crustycrab.util

import retrofit2.Response
import java.io.IOException

/**
 * Runs an API call, catches every possible failure mode, and
 * returns a clean ApiResult. The calling code never sees an
 * exception, an HTTP code, or a null body.
 */
suspend fun <T : Any> safeApiCall(
    call: suspend () -> Response<T>
): ApiResult<T> {
    return try {
        val response = call()
        val body = response.body()

        if (response.isSuccessful && body != null) {
            ApiResult.Success(body)
        } else {
            // Build a friendlier message than "HTTP 500: Internal Server Error"
            val code = response.code()
            val fallback = when (code) {
                400 -> "Invalid request"
                401 -> "Wrong email or password"
                403 -> "Not allowed"
                404 -> "Not found"
                409 -> "Already exists"
                500, 502, 503 -> "Server error. Try again later."
                else -> "Request failed (HTTP $code)"
            }
            ApiResult.Error(fallback)
        }
    } catch (e: IOException) {
        ApiResult.Error("No connection. Check your network.")
    } catch (e: Exception) {
        ApiResult.Error("Unexpected error: ${e.message ?: "unknown"}")
    }
}

/**
 * Variant for calls that don't return a body (204 No Content,
 * or Unit endpoints like DELETE).
 */
suspend fun safeApiCallUnit(
    call: suspend () -> Response<Unit>
): ApiResult<Unit> {
    return try {
        val response = call()
        if (response.isSuccessful) {
            ApiResult.Success(Unit)
        } else {
            ApiResult.Error("Request failed (HTTP ${response.code()})")
        }
    } catch (e: IOException) {
        ApiResult.Error("No connection. Check your network.")
    } catch (e: Exception) {
        ApiResult.Error("Unexpected error: ${e.message ?: "unknown"}")
    }
}
