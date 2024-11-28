plugins {
    id("java")
}

group = "alexandria"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
}

dependencies {
    testImplementation(platform("org.junit:junit-bom:5.10.0"))
    testImplementation("org.junit.jupiter:junit-jupiter")
    compileOnly("org.keycloak:keycloak-server-spi-private:26.0.6")
    compileOnly("org.keycloak:keycloak-server-spi:26.0.6")
    compileOnly("org.keycloak:keycloak-core:26.0.6")
}

tasks.test {
    useJUnitPlatform()
}