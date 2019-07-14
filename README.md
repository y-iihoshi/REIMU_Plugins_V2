# REIMU_Plugins_V2

REIMU plugins for Touhou replays (Redesigning)

[![Build status](https://ci.appveyor.com/api/projects/status/isxeiqhusbvoolev?svg=true)](https://ci.appveyor.com/project/y-iihoshi/reimu-plugins-v2)
[![License](https://img.shields.io/github/license/y-iihoshi/REIMU_Plugins_V2.svg)](https://github.com/y-iihoshi/REIMU_Plugins_V2/blob/master/LICENSE.txt)

## Introduction

This is the plugin set for [REIMU][REIMU], the file viewer extendable by various
plugins, to list Touhou game's replay files and/or bestshot files.

This project, written in C#, is a redesigning version of
[the previous project][V1], written in C++.

## Supported Titles

### Supported
* TH09.5 東方文花帖　～ Shoot the Bullet. (Bestshots only)
* TH10.5 東方緋想天　～ Scarlet Weather Rhapsody.
* TH12 東方星蓮船　～ Undefined Fantastic Object.
* TH12.3 東方非想天則　～ 超弩級ギニョルの謎を追え (a.k.a. Touhou Hisoutensoku)
* TH12.5 ダブルスポイラー　～ 東方文花帖 (a.k.a. Double Spoiler; Replays and
  bestshots)
* TH12.8 妖精大戦争　～ 東方三月精 (a.k.a. Fairy Wars)
* TH13 東方神霊廟　～ Ten Desires.
* TH13.5 東方心綺楼　～ Hopeless Masquerade.
* TH14 東方輝針城　～ Double Dealing Character.
* TH14.3 弾幕アマノジャク　～ Impossible Spell Card. (Replays and screenshots)
* TH14.5 東方深秘録　～ Urban Legend in Limbo.
* TH15 東方紺珠伝　～ Legacy of Lunatic Kingdom.
* TH15.5 東方憑依華　～ Antinomy of Common Flowers.
* Th16 東方天空璋　～ Hidden Star in Four Seasons.
* Th16.5 秘封ナイトメアダイアリー　～ Violet Detector.

### Will be supported
* Th17 東方鬼形獣　～ Wily Beast and Weakest Creature.

### Not supported
The followings are not supported because these plugins have already been
released by [sue445][sue445], the REIMU developer:
* TH07.5 東方萃夢想　～ Immaterial and Missing Power.
* TH08 東方永夜抄　～ Imperishable Night.
* TH09 東方花映塚　～ Phantasmagoria of Flower View.
* TH09.5 東方文花帖　～ Shoot the Bullet. (Replays only)
* TH10 東方風神録　～ Mountain of Faith.
* TH11 東方地霊殿　～ Subterranean Animism. (But implemented for my practice)

The followings are not supported by the same reason that why sue445 doesn't
release these plugins:
* TH06 東方紅魔郷　～ Embodiment of Scarlet Devil.
* TH07 東方妖々夢　～ Perfect Cherry Blossom.

The older versions of every title are not supported because I can't get all and
test enough.

## Environments

### Development environment
* Windows 10 Pro Version 1903 (64bit)
* .NET Framework 4.8
* Visual Studio Community 2019 16.1.6
* Python 3.7.4

### Tested environment
Same as above but you can use .NET Framework 4.5.2 or later.

## Installation

    <Top directory>\
     |-- reimu.chm
     |-- reimu.exe
     |-- reimu.ini
     |-- CommonWin32.dll          <-- Place here.
     |-- ReimuPlugins.Common.dll  <-- Place here.
     `-- plugin\
          `-- ReimuPlugins.*.rpi  <-- Place here. Subdirectory/-ies can be created.

## How to Use

See the REIMU manual (reimu.chm; Japanese only).

## Notes

Sorry, all plugins have Japanese resource only and don't support localization.

[ZUN]: http://www16.big.or.jp/~zun/ "上海アリス幻樂団"
[tasofro]: http://www.tasofro.net/ "Twilight-Frontier"
[sue445]: http://www.sue445.net/ "sue445.NET"
[REIMU]: http://www.sue445.net/downloads/reimu.html "REIMU (REplayviewer plug-in IMport Utility)"
[V1]: https://github.com/y-iihoshi/REIMU_Plugins "REIMU_Plugins"
