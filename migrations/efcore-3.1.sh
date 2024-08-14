#!/usr/bin/env bash

# Define the modules to update
mods=("Alterations" "Runtime" "Management" "Identity" "Labels")

# Define the list of providers
providers=("MySql" "SqlServer" "Sqlite" "PostgreSql" "Oracle")

# Loop through each module
for module in "${mods[@]}"; do
    # Loop through each provider
    for provider in "${providers[@]}"; do
        providerPath="../src/modules/Elsa.EntityFrameworkCore.$provider"
        migrationsPath="Migrations/$module"
    
        echo "Updating migrations for $provider..."
        echo "Provider path: ${providerPath:?}/${migrationsPath}"
        echo "Migrations path: $migrationsPath"

        # Run the migrations command using version 1.0.1 of ef-migration-runtime-schema
        dotnet-ef-core-runtime-schema --interface Elsa.EntityFrameworkCore.Common.Contracts.IElsaDbContextSchema -- migrations add V3_1 -c ""$module""ElsaDbContext -p ""$providerPath""  -o ""$migrationsPath""
    done
done
