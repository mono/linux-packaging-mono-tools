#
# spec file for package mono-tools
#
# Copyright (c) 2013 SUSE LINUX Products GmbH, Nuernberg, Germany.
#
# All modifications and additions to the file contributed by third parties
# remain the property of their copyright owners, unless otherwise agreed
# upon. The license for this file, and modifications and additions to the
# file, is the same license as for the pristine package itself (unless the
# license for the pristine package is not an Open Source License, in which
# case the license is the MIT License). An "Open Source License" is a
# license that conforms to the Open Source Definition (Version 1.9)
# published by the Open Source Initiative.

# Please submit bugfixes or comments via http://bugs.opensuse.org/
#


Name:           mono-tools
Version:	4.2
Release:	0.xamarin.7
BuildArch:      noarch
Url:            http://go-mono.org/
Source0:        %{name}-%{version}.tar.gz
Summary:        Collection of Tools and Utilities for Mono
License:        GPL-2.0 and LGPL-2.0 and MIT
Group:          Development/Tools/Other
BuildRoot:      %{_tmppath}/%{name}-%{version}-build
BuildRequires:  autoconf
BuildRequires:  automake
BuildRequires:  gettext
BuildRequires:  libtool
BuildRequires:  glib2-devel
BuildRequires:  pkgconfig(glib-sharp-2.0) >= 2.12.40
BuildRequires:  pkgconfig(glade-sharp-2.0) >= 2.12.40
BuildRequires:  pkgconfig(gnome-sharp-2.0) >= 2.24.3
BuildRequires:  pkgconfig(mono) >= 5.0
BuildRequires:  pkgconfig(mono-nunit) >= 5.0
BuildRequires:  pkgconfig(monodoc) >= 5.0
BuildRequires:  desktop-file-utils
BuildRequires:  pkgconfig(webkit-sharp-1.0)

%description
Mono Tools is a collection of development and testing programs and
utilities for use with Mono.

%define _use_internal_dependency_generator 0
%if 0%{?fedora} || 0%{?rhel} || 0%{?centos}
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/redhat/find-provides && printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/redhat/find-requires && printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/mono-find-requires ; } | sort | uniq'
%else
%define __find_provides env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/find-provides && printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/mono-find-provides ; } | sort | uniq'
%define __find_requires env sh -c 'filelist=($(cat)) && { printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/find-requires && printf "%s\\n" "${filelist[@]}" | %{_prefix}/lib/rpm/mono-find-requires ; } | sort | uniq'
%endif

%files -f %{name}.lang
%defattr(-, root, root)
%if 0%{?fedora}
%_bindir/gasnview
%_prefix/lib/mono/1.0/gasnview.exe
%endif
%_bindir/create-native-map
%_bindir/emveepee
%_bindir/gendarme
%_bindir/gendarme-wizard
%_bindir/gd2i
%_bindir/gsharp
%_bindir/gui-compare
%_bindir/ilcontrast
%_bindir/minvoke
%_bindir/monodoc
%_bindir/mperfmon
%_bindir/mprof-decoder
%_bindir/mprof-heap-viewer
%_datadir/applications/gendarme-wizard.desktop
%_datadir/applications/gsharp.desktop
%_datadir/applications/ilcontrast.desktop
%_datadir/applications/monodoc.desktop
%_datadir/create-native-map
%_datadir/icons/hicolor/*/apps/monodoc.png
%_datadir/pixmaps/gendarme.svg
%_datadir/pixmaps/ilcontrast.png
%_datadir/pixmaps/monodoc.png
%_datadir/pkgconfig/create-native-map.pc
%_datadir/pkgconfig/gendarme-framework.pc
%_mandir/man1/create-native-map*
%_mandir/man1/gd2i*
%_mandir/man1/gendarme*
%_mandir/man1/mperfmon*
%_mandir/man1/mprof-decoder*
%_mandir/man1/mprof-heap-viewer*
%_mandir/man5/gendarme*
%_prefix/lib/create-native-map
%_prefix/lib/gendarme
%_prefix/lib/gsharp
%_prefix/lib/gui-compare
%_prefix/lib/ilcontrast
%_prefix/lib/minvoke
%_prefix/lib/mono-tools
%_prefix/lib/monodoc/*.dll*
%_prefix/lib/monodoc/*.exe*
%_prefix/lib/monodoc/sources
%_prefix/lib/mperfmon

%package -n monodoc-http
Summary:        ASP.NET front-end for displaying Monodoc documentation
License:        MIT
Group:          Development/Tools/Other

%description -n monodoc-http
Monodoc-http provides an ASP.NET front-end for displaying installed 
Monodoc documentation.

%files -n monodoc-http
%defattr(-, root, root)
%_prefix/lib/monodoc/web

%prep
%setup -q

%build
autoreconf -vi
# Cannot use the configure macro because noarch-redhat-linux is not recognized by the auto tools in the tarball
./configure --prefix=%{_prefix} \
	    --libexecdir=%{_prefix}/lib \
	    --libdir=%{_prefix}/lib \
	    --mandir=%{_mandir} \
	    --infodir=%{_infodir} \
	    --sysconfdir=%{_sysconfdir}

%install
make install DESTDIR=%{buildroot}

# Move create-native-map stuff out of lib into share
mkdir %{buildroot}/%_prefix/share/create-native-map
mv %{buildroot}/%_prefix/lib/create-native-map/MapAttribute.cs %{buildroot}/%_prefix/share/create-native-map
mv %{buildroot}/%_prefix/lib/pkgconfig %{buildroot}/%_prefix/share

desktop-file-install --dir=%{buildroot}/%{_datadir}/applications docbrowser/monodoc.desktop
desktop-file-install --dir=%{buildroot}/%{_datadir}/applications ilcontrast/ilcontrast.desktop
desktop-file-install --dir=%{buildroot}/%{_datadir}/applications gendarme/swf-wizard-runner/gendarme-wizard.desktop
desktop-file-install --dir=%{buildroot}/%{_datadir}/applications gsharp/gsharp.desktop

%find_lang %{name}

%post
%{_bindir}/monodoc --make-index

%changelog

