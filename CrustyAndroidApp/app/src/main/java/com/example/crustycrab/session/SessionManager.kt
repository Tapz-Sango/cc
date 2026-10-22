package com.example.crustycrab.session

import android.content.Context
import android.content.SharedPreferences

object SessionManager {

    private const val PREFS = "crusty_session"
    private const val KEY_USER_ID = "user_id"
    private const val KEY_EMAIL = "user_email"
    private const val KEY_NAME = "user_name"
    private const val KEY_ROLE = "user_role"
    private const val KEY_LOGGED_IN = "logged_in"

    private fun prefs(ctx: Context): SharedPreferences =
        ctx.applicationContext.getSharedPreferences(PREFS, Context.MODE_PRIVATE)

    fun saveLogin(
        ctx: Context,
        userId: Int,
        email: String,
        name: String,
        role: String
    ) {
        prefs(ctx).edit()
            .putInt(KEY_USER_ID, userId)
            .putString(KEY_EMAIL, email)
            .putString(KEY_NAME, name)
            .putString(KEY_ROLE, role)
            .putBoolean(KEY_LOGGED_IN, true)
            .apply()
    }

    fun isLoggedIn(ctx: Context): Boolean =
        prefs(ctx).getBoolean(KEY_LOGGED_IN, false)

    fun getUserId(ctx: Context): Int =
        prefs(ctx).getInt(KEY_USER_ID, -1)

    fun getEmail(ctx: Context): String =
        prefs(ctx).getString(KEY_EMAIL, "") ?: ""

    fun getName(ctx: Context): String =
        prefs(ctx).getString(KEY_NAME, "") ?: ""

    fun getRole(ctx: Context): String =
        prefs(ctx).getString(KEY_ROLE, "") ?: ""

    fun clear(ctx: Context) {
        prefs(ctx).edit().clear().apply()
    }
}
