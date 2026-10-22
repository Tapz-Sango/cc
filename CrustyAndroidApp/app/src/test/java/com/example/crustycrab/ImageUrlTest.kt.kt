package com.example.crustycrab

import org.junit.Assert.assertEquals
import org.junit.Test

class ImageUrlTest {

    private fun buildImageUrl(path: String?): String? {
        if (path.isNullOrBlank()) {
            return null
        }

        return if (path.startsWith("http")) {
            path
        } else {
            "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net" +
                    if (path.startsWith("/")) {
                        path
                    } else {
                        "/$path"
                    }
        }
    }

    @Test
    fun imagePath_withLeadingSlash_buildsCorrectUrl() {
        val result = buildImageUrl("/Images/Menu/OceanBite2.png")

        assertEquals(
            "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/Images/Menu/OceanBite2.png",
            result
        )
    }

    @Test
    fun imagePath_withoutLeadingSlash_addsSlash() {
        val result = buildImageUrl("Images/Menu/OceanBite2.png")

        assertEquals(
            "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/Images/Menu/OceanBite2.png",
            result
        )
    }

    @Test
    fun nullImagePath_returnsNull() {
        assertEquals(null, buildImageUrl(null))
    }

    @Test
    fun fullUrl_isNotChanged() {
        val url =
            "https://crusty-crab-api-tameez-hjechmamdneee7c6.southafricanorth-01.azurewebsites.net/Images/Menu/OceanBite2.png"

        assertEquals(url, buildImageUrl(url))
    }
}