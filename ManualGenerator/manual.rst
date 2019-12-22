.. REIMU Plugins V2 documentation master file, created by
   sphinx-quickstart on Sat Aug 26 21:06:35 2017.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

REIMU Plugins V2 for Touhou replays User's Manual
=================================================

.. toctree::
   :maxdepth: 2


イントロダクション
------------------

これは何？
^^^^^^^^^^

これは、 プラグイン型リプレイビュアー
`REIMU <http://www.sue445.net/downloads/reimu.html>`_ 用のプラグインセットです。

REIMU に導入することで、 東方 Project
の各作品のリプレイやベストショットなどの一覧を表示できるようになります。
リプレイのコメントの編集やベストショットの表示もできます。

前提条件
^^^^^^^^

本プラグインセットを利用するには .NET Framework 4.8 が必要です。

2019 年 12 月現在、
本プラグインセットは下記環境でのみ開発及び動作確認を実施しています。
下記以外の環境で動作しないなどのご報告を受けても対応できない可能性があります。

* Windows 10 Pro Version 1909 (64bit)
* .NET Framework 4.8
* Visual Studio Community 2019 Version 16.4.2
* Python 3.8.0

免責事項
^^^^^^^^

**本プラグインセットの利用は自己責任の下で実施してください。**

本プラグインセットの免責事項などの詳細は以下の通りです。
(つまり BSD 2-Clause License です。)

.. literalinclude:: ../LICENSE.txt
   :language: none

著作権
^^^^^^

* 東方 Project の各作品の著作権は
  `上海アリス幻樂団 <http://www16.big.or.jp/~zun/>`_ 様にあります。
  本プラグインセット作者とは一切関係がありません。
* 東方萃夢想、 東方緋想天、 東方非想天則、 東方心綺楼、 東方深秘録の著作権は
  `黄昏フロンティア <http://www.tasofro.net/>`_ 様及び
  `上海アリス幻樂団 <http://www16.big.or.jp/~zun/>`_ 様にあります。
  本プラグインセット作者とは一切関係がありません。
* REIMU の著作権は
  `sue445 <http://www.sue445.net/>`_ 様にあります。
  本プラグインセット作者とは一切関係がありません。
* 本プラグインセットの著作権は IIHOSHI Yoshinori
  (`Web サイト <https://www.colorless-sight.jp/>`_,
  `Twitter <https://twitter.com/iihoshi>`_) にあります。


各作品への対応状況
------------------

対応済み
^^^^^^^^

本プラグインセットに含まれる各プラグインが対応している、
東方 Project の作品名及びバージョンは以下の通りです:

================================ ======================================================
プラグイン名                     対応作品名及びバージョン
================================ ======================================================
ReimuPlugins.Th095Bestshot.rpi   東方文花帖 ver. 1.02a (ベストショット用)
ReimuPlugins.Th105Replay.rpi     東方緋想天 ver. 1.06a (リプレイ用)
ReimuPlugins.Th12Replay.rpi      東方星蓮船 ver. 1.00b (リプレイ用)
ReimuPlugins.Th123Replay.rpi     東方非想天則 ver. 1.10a (リプレイ用)
ReimuPlugins.Th125Replay.rpi     ダブルスポイラー ver. 1.00a (リプレイ用)
ReimuPlugins.Th125Bestshot.rpi   ダブルスポイラー ver. 1.00a (ベストショット用)
ReimuPlugins.Th128Replay.rpi     妖精大戦争 ver. 1.00a (リプレイ用)
ReimuPlugins.Th13Replay.rpi      東方神霊廟 ver. 1.00c (リプレイ用)
ReimuPlugins.Th135Replay.rpi     東方心綺楼 ver. 1.34b (リプレイ用)
ReimuPlugins.Th14Replay.rpi      東方輝針城 ver. 1.00b (リプレイ用)
ReimuPlugins.Th143Replay.rpi     弾幕アマノジャク ver. 1.00a (リプレイ用)
ReimuPlugins.Th143Screenshot.rpi 弾幕アマノジャク ver. 1.00a (スクリーンショット用)
ReimuPlugins.Th145Replay.rpi     東方深秘録 ver. 1.41 (リプレイ用)
ReimuPlugins.Th15Replay.rpi      東方紺珠伝 ver. 1.00b (リプレイ用)
ReimuPlugins.Th155Replay.rpi     東方憑依華 ver. 1.10c (リプレイ用)
ReimuPlugins.Th16Replay.rpi      東方天空璋 ver. 1.00a (リプレイ用)
ReimuPlugins.Th165Replay.rpi     秘封ナイトメアダイアリー ver. 1.00a (リプレイ用)
ReimuPlugins.Th165Bestshot.rpi   秘封ナイトメアダイアリー ver. 1.00a (ベストショット用)
ReimuPlugins.Th17Replay.rpi      東方鬼形獣 ver. 1.00b (リプレイ用)
================================ ======================================================

対応予定あり
^^^^^^^^^^^^

以下作品については、
本プラグインセット作者の原作プレイ進捗状況に伴って今後対応予定です:

* 東方剛欲異聞

対応予定なし
^^^^^^^^^^^^

以下作品については、 REIMU 作者謹製プラグインがあるので、 対応予定はありません:

* 東方萃夢想
* 東方永夜抄
* 東方花映塚
* 東方文花帖
* 東方風神録
* 東方地霊殿

以下作品については、 REIMU 作者が公開していないのと同じ理由により、
対応予定はありません：

* 東方紅魔郷
* 東方妖々夢

全作品の過去バージョンで作成されたリプレイファイルについては、
本プラグインセット作者の方での動作確認が困難なので、 対応予定はありません。


インストレーション
------------------

インストール方法
^^^^^^^^^^^^^^^^

Web からダウンロードした zip ファイルを任意の場所に展開して下さい。
展開後のファイル構成は以下のようになります。 ::

    <展開先フォルダー>\
      |-- doc\                      (マニュアル一式)
      |     |-- *.html
      |     `-- ...
      |-- plugin\
      |     `-- ReimuPlugins.*.rpi  (REIMU 用プラグイン一式)
      |-- CommonWin32.dll           (ベストショット用 *.rpi に必要な DLL)
      `-- ReimuPlugins.Common.dll   (全ての *.rpi に必要な DLL)

これらのファイルを、 以下のファイル構成になるようにそれぞれ格納して下さい。 ::

    <REIMU 本体のフォルダー>\
      |-- plugin\
      |     |-- ReimuPlugins.*.rpi  <--
      |     `-- ...
      |-- CommonWin32.dll           <--
      |-- ReimuPlugins.Common.dll   <--
      |-- reimu.chm
      |-- reimu.exe
      `-- reimu.ini

アップデート方法
^^^^^^^^^^^^^^^^

Ver. 20140824 以前から Ver. 20170827 以降にアップデートする場合は、
以前の \*.rpi ファイル (th12replay.rpi など) を削除して下さい。

アンインストール方法
^^^^^^^^^^^^^^^^^^^^

格納した \*.dll ファイルや \*.rpi ファイルを削除して下さい。
本プラグインセットはレジストリの書き換えをしません。


使い方
------

基本的に REIMU 作者謹製プラグインと同様ですので、 そちらの readme.txt や
REIMU 本体のヘルプを参照してください。


注意点
------

以下の注意点は、 今後のアップデートにより変更される可能性があります。

* プラグインごとの設定 (コメントの表記方法を変えるなど) は、
  今のところ対応予定はありません。
* REIMU 本体の
  「リストビュー作成時に最近読み込んだフォルダの履歴を自動的に読み込む」
  設定を有効にしていても、
  地霊殿以降の作品については読み込んでくれないようです。 (REIMU 本体の不具合？)
  REIMU 本体の設定ファイル (reimu.ini) 内の
  ``RecentDir1`` を手動で設定すれば読み込むようになります。


変更履歴
--------

Ver. 20191222
    * 東方鬼形獣に対応
    * .NET Framework の対象バージョンを 4.5.2 から 4.8 に変更

Ver. 20190415
    * 秘封ナイトメアダイアリーに対応

      * ReimuPlugins.Th165Replay.rpi: リプレイファイル用
      * ReimuPlugins.Th165Bestshot.rpi: ベストショットファイル用

Ver. 20180504
    * 東方憑依華に対応
    * .NET Framework の対象バージョンを 4.0 から 4.5.2 に変更

Ver. 20170924
    * 東方天空璋に対応

Ver. 20170827
    * 全て C# で再実装
    * マニュアルの作成に `Sphinx <http://www.sphinx-doc.org/ja/stable/>`_
      を利用するように変更
    * 東方心綺楼に対応
    * 東方深秘録に対応
    * 東方紺珠伝に対応

Ver. 20140824
    * 東方緋想天に対応
    * 東方非想天則に対応

Ver. 20140610
    * 弾幕アマノジャクに対応

      * th143replay.rpi: リプレイファイル用
      * th143screenshot.rpi: スクリーンショットファイル用

Ver. 20140302
    * 東方輝針城に対応

Ver. 20140131
    * 東方神霊廟に対応
    * 全体的にソースコードを整理

Ver. 20131210
    * 妖精大戦争に対応

Ver. 20131116
    * 東方文花帖に対応

      * th095bestshot.rpi: ベストショットファイル用

Ver. 20131110
    * ダブルスポイラーに対応

      * th125replay.rpi: リプレイファイル用
      * th125bestshot.rpi: ベストショットファイル用

Ver. 20130929\_2
    * Windows XP への対応漏れを修正
    * このマニュアルを追加

Ver. 20130929
    * 公開開始
    * 東方星蓮船に対応
