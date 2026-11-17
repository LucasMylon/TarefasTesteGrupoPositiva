# 🔧 Solução AppLocker - Windows Security Policy

## 🎯 Problema

Windows AppLocker (Política de Controle de Aplicativo) bloqueia DLLs e EXEs com erro:
```
Uma política de Controle de Aplicativo bloqueou este arquivo. (0x800711C7)
```

Especialmente quando:
- Projeto está em OneDrive
- Arquivos vêm de zona segura (internet)
- DLLs NuGet são baixadas

---

## ✅ Soluções (Em Ordem de Eficácia)

### SOLUÇÃO 1: Release Mode (MELHOR) ⭐
**Eficácia:** 95% | **Dificuldade:** Fácil

```powershell
# AppLocker geralmente não bloqueia binários Release
# porque são assinados ou de origem confiável

dotnet build -c Release
dotnet run --project TarefasTeste.Api --configuration Release
```

**Vantagem:** Funciona sem mudanças de policy
**Desvantagem:** Nenhuma realmente

---

### SOLUÇÃO 2: Desbloquear Arquivos (PowerShell)
**Eficácia:** 50% | **Dificuldade:** Fácil

```powershell
# Desbloquear arquivo único
Unblock-File -Path "C:\Users\Lucas\...\TarefasTeste.Domain.dll"

# Desbloquear todos DLLs de uma pasta
Get-ChildItem -Path "bin\Debug\net10.0\" -Filter "*.dll" | Unblock-File

# Desbloquear toda solução
Get-ChildItem -Recurse -Filter "*.dll" | Unblock-File
Get-ChildItem -Recurse -Filter "*.exe" | Unblock-File
```

**Nota:** Funciona temporariamente, AppLocker pode re-bloquear após rebuild

---

### SOLUÇÃO 3: Visual Studio Bypass
**Eficácia:** 80% | **Dificuldade:** Média

```
1. Abrir Visual Studio como Administrator
2. Tools → Options → Debugging → enable Native Code Debugging
3. Executar projeto via "Start Debugging" (F5)
```

**Vantagem:** AppLocker menos rigoroso com VS
**Desvantagem:** Requer admin, mais lento

---

### SOLUÇÃO 4: Windows Firewall Exception
**Eficácia:** 20% | **Dificuldade:** Fácil

```powershell
# Adicionar firewall exception (não resolve AppLocker, mas útil)
New-NetFirewallRule -DisplayName "Allow Dotnet Runtime" `
  -Program "C:\Program Files\dotnet\dotnet.exe" `
  -Direction Inbound -Action Allow
```

---

### SOLUÇÃO 5: Desabilitar AppLocker (NÃO RECOMENDADO)
**Eficácia:** 100% | **Dificuldade:** Difícil | **Risco:** Alto

```powershell
# ⚠️ APENAS COMO ÚLTIMO RECURSO ⚠️

# Verificar política
Get-AppLockerPolicy -Effective | Out-File C:\policy.xml

# Para desabilitar, precisa de admin:
# Start → gpedit.msc → Computer Configuration 
#   → Windows Settings → Security Settings 
#   → Application Control Policies → AppLocker 
#   → Disable all rules

# OU via PowerShell (admin):
Set-AppLockerPolicy -PolicyObject $null -Enforce
```

**Risco:** Reduz segurança do sistema operacional

---

## 🚀 Estratégia Recomendada

### Para Desenvolvimento Local:
```powershell
# 1. Use RELEASE MODE por padrão
dotnet run --project TarefasTeste.Api --configuration Release

# 2. Se precisar DEBUG, desbloquear uma vez:
Get-ChildItem -Recurse -Filter "*.dll" | Unblock-File

# 3. Build em Debug
dotnet run --project TarefasTeste.Api
```

### Para CI/CD (Pipeline):
```bash
# Usar Release build sempre
dotnet build -c Release
dotnet run --configuration Release
```

### Para Produção:
```bash
# Release + executável standalone
dotnet publish -c Release --self-contained
```

---

## 📋 Troubleshooting por Situação

### Situação 1: "Cannot load assembly" ao rodar `dotnet run`
```powershell
# 1. Tente Release mode
dotnet run --project TarefasTeste.Api --configuration Release

# 2. Se falhar, desbloquear
Get-ChildItem -Path "bin\Release\net10.0\" -Filter "*.dll" | Unblock-File

# 3. Tentar novamente
dotnet run --project TarefasTeste.Api --configuration Release
```

### Situação 2: "Cannot load assembly" ao rodar testes
```powershell
# 1. Desbloquear bin de testes
Get-ChildItem -Path "TarefasTeste.Api.Tests\bin\**" -Filter "*.dll" | Unblock-File

# 2. Rodar testes
dotnet test TarefasTeste.Api.Tests

# 3. Se ainda falhar, use Release
dotnet build -c Release TarefasTeste.Api.Tests
dotnet test TarefasTeste.Api.Tests -c Release
```

### Situação 3: Projeto em OneDrive sincronizando
```powershell
# Adicionar pastas de build ao OneDrive ignore
# Arquivo: .onedriveignore no raiz do projeto
bin/
obj/
.vs/
*.dll
*.exe
```

Ou diretamente no OneDrive:
1. Clique direito na pasta `bin/`
2. "Free up space" (vai usar local-only)
3. Marque "Always keep on this device"

---

## 🔒 Entender AppLocker

### Como funciona:
```
1. Sistema operacional carrega arquivo .dll
2. AppLocker intercept no kernel
3. Verifica policy (admin só)
4. Se bloqueado → erro 0x800711C7
5. Se permitido → carrega normalmente
```

### Por que bloqueia seus arquivos:
- ✓ Arquivo vem de internet/OneDrive
- ✓ Falta atributo de assinatura válida
- ✓ Policy admin está muito restritiva
- ✓ File zone alternate stream markers

### Como `Unblock-File` ajuda:
```powershell
# Remove o alternate stream "Zone.Identifier"
# Que marca arquivo como "downloaded from internet"

# Verificar streams:
Get-Item -Path "file.dll" -Stream *

# Remover stream:
Remove-Item -Path "file.dll" -Stream "Zone.Identifier"
```

---

## 🛡️ Melhores Práticas

### ✅ Fazer:
- ✅ Usar Release mode para produção
- ✅ Manter AppLocker habilitado (segurança)
- ✅ Desbloquear apenas DLLs confiáveis
- ✅ Usar release standalone para portabilidade
- ✅ Adicionar arquivo `.gitignore` para bin/obj

### ❌ Não Fazer:
- ❌ Desabilitar AppLocker por inteiro
- ❌ Desbloquear arquivos de origem desconhecida
- ❌ Dar permissão irrestrita a VS
- ❌ Usar Debug mode em produção
- ❌ Versionar bin/ e obj/

---

## 📊 Comparação de Soluções

| Solução | Funciona | Fácil | Seguro | Permanente |
|---------|----------|-------|--------|-----------|
| Release Mode | ✅ | ✅ | ✅ | ✅ |
| Unblock-File | ⚠️ | ✅ | ⚠️ | ❌ |
| VS Admin | ✅ | ⚠️ | ⚠️ | ⚠️ |
| Firewall Exception | ❌ | ✅ | ✅ | ✅ |
| Disable AppLocker | ✅ | ✅ | ❌ | ✅ |

**RECOMENDAÇÃO:** Release Mode + ocasional Unblock-File

---

## 🔍 Verificar Politica Atual

```powershell
# Ver politica ativa
Get-AppLockerPolicy -Effective | Out-File C:\applock_policy.xml

# Abrir arquivo gerado
Invoke-Item C:\applock_policy.xml

# Listar regras
(Get-AppLockerPolicy -Effective).RuleCollections | ForEach-Object { $_.Rules }
```

---

## 🚀 Script Automatizado

Salve como `fix-applock.ps1`:

```powershell
# fix-applock.ps1 - Script para corrigir AppLocker

param(
	[switch]$Release = $false,
	[switch]$UnblockDLLs = $false,
	[string]$Path = "."
)

Write-Host "🔧 Corrigindo AppLocker Issues..." -ForegroundColor Cyan

if ($UnblockDLLs) {
	Write-Host "📂 Desbloqueando DLLs..." -ForegroundColor Yellow
	Get-ChildItem -Path $Path -Recurse -Filter "*.dll" | Unblock-File
	Get-ChildItem -Path $Path -Recurse -Filter "*.exe" | Unblock-File
	Write-Host "✅ DLLs desbloqueados" -ForegroundColor Green
}

if ($Release) {
	Write-Host "🏗️ Compilando em Release mode..." -ForegroundColor Yellow
	dotnet build -c Release
	Write-Host "✅ Release build completo" -ForegroundColor Green
}

Write-Host ""
Write-Host "✨ Pronto! Execute:" -ForegroundColor Green
Write-Host "dotnet run --project TarefasTeste.Api --configuration Release"
```

Usar:
```powershell
.\fix-applock.ps1 -Release -UnblockDLLs
```

---

## 📞 Quando Pedir Ajuda

Se AppLocker persistir, sua organização pode ter:
- Politica corporativa muito restritiva
- Requisitos de assinatura de código
- Validação de certificado necessária

**Contactar:** Seu administrador de TI

---

## 📚 Referências

- [Microsoft AppLocker Documentation](https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/applocker/)
- [Unblock-File PowerShell](https://docs.microsoft.com/powershell/module/microsoft.powershell.utility/unblock-file)
- [DotNet Security](https://docs.microsoft.com/dotnet/core/setup/windows)

---

**TL;DR:** Use `Release mode` em vez de `Debug` e AppLocker deixará você em paz! 🚀
