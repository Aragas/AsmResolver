#!/bin/bash

# usage: install-wine-addon.sh { mono | gecko }

# keywords: wine-mono wine-gecko msi share/wine/mono share/wine/gecko

go() {
  local pkg="$1"
  local wineexe=$(readlink /usr/bin/wine)
  # e.g. /opt/wine-devel/bin/wine
  local winedir=$(dirname $(dirname $wineexe))
  local targetdir="$winedir/share/wine/$pkg"
  # e.g. /opt/wine-devel/lib64/wine/x86_64-windows/appwiz.cpl
  local appwiz="${winedir}/lib64/wine/x86_64-windows/appwiz.cpl"
  echo "==> Looking for 'wine-$pkg' in: $appwiz"
  local msi=$(strings -e l "$appwiz" | grep -o "wine-$pkg-.*msi")
  local pkgver="${msi##wine-$pkg-}"
  pkgver="${pkgver%%-*}"
  echo "==> wine-$pkg = $pkgver"
  test -z "$pkgver" && exit 1
  echo "==> Installing into: $targetdir ..."
  mkdir -p "$targetdir"
  wget -P "$targetdir" -N "https://dl.winehq.org/wine/wine-${pkg}/${pkgver}/wine-${pkg}-${pkgver}-x86.tar.xz" "https://dl.winehq.org/wine/wine-${pkg}/${pkgver}/wine-${pkg}-${pkgver}-x86.msi"
  #tar -xf wine-${pkg}-${pkgver}-x86.tar.xz
  #mv wine-${pkg}-${pkgver}-x86.msi wine-${pkg}-${pkgver}/
}

go "${1:-mono}"