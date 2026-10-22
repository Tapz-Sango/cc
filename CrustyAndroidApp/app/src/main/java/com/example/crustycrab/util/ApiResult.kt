package com.example.crustycrab.util

/**
 * Wrapper for every API call. The UI never has to think about
 * HTTP status codes, exceptions, or null bodies ΓÇö it just gets
 * a Success or Error and moves on.
 */
sealed class ApiResult<out T> {

    data class Success<T>(val data: T) : ApiResult<T>()

    data class Error(val message: String) : ApiResult<Nothing>()
}
