package com.example.crustycrab.api

object ApiConfig {
    /**
     * Base URL for the deployed CrustyAPI backend.
     *
     * The trailing slash is REQUIRED by Retrofit.
     */
    const val BASE_URL = "http://10.0.2.2:5083/"

    const val TIMEOUT_SECONDS = 30L
}