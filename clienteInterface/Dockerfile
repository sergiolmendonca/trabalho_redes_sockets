FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm AS base

RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 \
    libfreetype6 \
    libx11-6 \
    libxrandr2 \
    libxrender1 \
    libxi6 \
    libxtst6 \
    libxcb1 \
    libxkbcommon0 \
    libx11-xcb1 \
    libgl1 \
    libharfbuzz0b \
    libsm6 \
    libice6 \
    libxext6 \
    && rm -rf /var/lib/apt/lists/*