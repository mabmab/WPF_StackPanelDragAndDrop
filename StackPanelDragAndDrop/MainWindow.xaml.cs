using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace StackPanelDragAndDrop
{
    ///++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// メインウィンドウクラス
    /// </summary>
    ///++++++++++++++++++++++++++++++++++++++++++++++++++++
    public partial class MainWindow : Window
    {
        //=================================================
        // フィールド
        //=================================================

        #region フィールド
        /// <summary>ドラッグアイテム</summary>
        private ImageView _draggedItem = null;
        /// <summary>ドラッグアイテムインデックス</summary>
        private int? _draggedItemIndex = null;

        /// <summary>初期位置</summary>
        private Point? _initialPosition = null;
        /// <summary>マウス位置とアイテム位置のオフセット</summary>
        private Point _mouseOffsetFromItem;

        /// <summary>ドラッグコンテンツ装飾オブジェクト</summary>
        private DragUserControlAdorner _dragContentAdorner = null;
        /// <summary>挿入カーソル装飾オブジェクト</summary>
        private InsertionAdorner _insertionAdorner = null;
        #endregion

        //=================================================
        // コンストラクター
        //=================================================

        #region コンストラクター
        /// <summary>デフォルトコンストラクター/summary>
        public MainWindow()
        {
            InitializeComponent();

            // メインパネルに複数イメージを追加
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Sheep.GetHbitmap(), "Sheep"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Cat.GetHbitmap(), "Cat"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Buffalo.GetHbitmap(), "Buffalo"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Horse.GetHbitmap(), "Horse"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Monkey.GetHbitmap(), "Monkey"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.Wolf.GetHbitmap(), "Wolf"));
            xMainPanel.Children.Add(CreateBitmapViewBox(Properties.Resources.RaccoonDog.GetHbitmap(), "RaccoonDog"));
        }
        #endregion

        //=================================================
        // イベントハンドラー
        //=================================================

        #region イベントハンドラー
        /// <summary>マウス左ボタンダウンイベント</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            // マウス左ボタンダウン位置のアイテムがドラッグ対象外なら何もしない
            ImageView draggedItem = args.Source as ImageView;
            if (draggedItem == null) return;

            // マウスキャプチャーに成功した場合にドラッグを開始する
            if (xMainPanel.CaptureMouse())
            {
                // ドラッグアイテムとそのインデックスの保存
                _draggedItem = draggedItem;
                _draggedItemIndex = xMainPanel.Children.IndexOf(draggedItem);

                // 初期位置とマウス位置とアイテム位置のオフセットの保存
                _initialPosition = PointToScreen(args.GetPosition(this));
                _mouseOffsetFromItem = draggedItem.PointFromScreen(_initialPosition.Value);
            }
        }

        /// <summary>マウス移動イベント</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainPanelMouseMove(object sender, MouseEventArgs args)
        {
            // マウス左ボタンダウン前のマウス移動では何もしない
            if (_draggedItem == null || _initialPosition == null) return;

            // ドラッグ装飾が未生成の場合はドラッグ装飾を生成する
            if (_dragContentAdorner == null)
            {
                _dragContentAdorner = new DragUserControlAdorner(xMainPanel, _draggedItem, _mouseOffsetFromItem);
            }

            // マウス移動中のパネル上のポイントとスクリーン座標位置を取得
            Point ptMovePanel = args.GetPosition(xMainPanel);
            Point ptMoveScreen = PointToScreen(args.GetPosition(this));

            // ドロップターゲットアイテムを取得
            ImageView dropTargetItem = xMainPanel.GetChildElement(ptMovePanel) as ImageView;

            // ドラッグ装飾を移動
            _dragContentAdorner.SetScreenPosition(ptMoveScreen);

            // ドロップターゲットアイテムがなければ挿入カーソル装飾を破棄する
            if (dropTargetItem == null)
            {
                _insertionAdorner?.Detach();
                _insertionAdorner = null;
            }
            // ドロップターゲットアイテムと挿入カーソル装飾のアイテムが違う場合は挿入カーソル装飾を再生成する
            else if (!dropTargetItem.Equals(_insertionAdorner?.AdornedElement))
            {
                _insertionAdorner?.Detach();

                // ドラッグアイテムとドロップターゲットアイテムの位置を取得
                int indexDraggedItem = xMainPanel.Children.IndexOf(_draggedItem);
                int indexDropTargetItem = xMainPanel.Children.IndexOf(dropTargetItem);
                // 挿入カーソル装飾を生成
                _insertionAdorner =
                    // ドラッグアイテムとドロップターゲットアイテム同じの場合は挿入カーソル装飾を表示しない
                    (indexDraggedItem == indexDropTargetItem) ? null :
                        // 後にドラッグする場合は右側、前にドラッグする場合は左側に挿入カーソル装飾を表示する
                        (indexDraggedItem < indexDropTargetItem) ?
                            new InsertionAdorner(dropTargetItem, true) :
                            new InsertionAdorner(dropTargetItem, false);
            }
            // ドロップターゲットアイテムと挿入カーソル装飾のアイテムが同じ場合は挿入カーソル装飾は変更しない
            else
            {
            }
        }

        /// <summary>マウス左ボタンアップイベント</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            // 左ボタンアップのポイントのアイテムを取得
            Point ptUp = args.GetPosition(xMainPanel);
            ImageView dropTargetItem = xMainPanel.GetChildElement(ptUp) as ImageView;
            
            // ドラッグ中、かつ、ドロップ位置のアイテムがドロップ対象ならアイテムを移動する
            if (_draggedItem !=null && dropTargetItem != null)
            {
                // アイテムの移動
                int dropTargetItemIndex = xMainPanel.Children.IndexOf(dropTargetItem);
                ImageView draggedItem = xMainPanel.Children[_draggedItemIndex.Value] as ImageView;
                xMainPanel.Children.RemoveAt(_draggedItemIndex.Value);
                xMainPanel.Children.Insert(dropTargetItemIndex, draggedItem);
            }

            // マウスキャプチャーのリリース
            xMainPanel.ReleaseMouseCapture();
        }

        /// <summary>マウスキャプチャーロストイベント</summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnMainPanelLostMouseCapture(object sender, MouseEventArgs args)
        {
            // 装飾のデタッチ
            _dragContentAdorner?.Detach();
            _dragContentAdorner = null;
            _insertionAdorner?.Detach();
            _insertionAdorner = null;

            // フィールドの初期化
            _draggedItem = null;
            _draggedItemIndex = null;
            _initialPosition = null;
        }
        #endregion

        //=================================================
        // プライベートメソッド
        //=================================================

        #region プライベートメソッド
        /// <summary>ビットマップ表示Viewboxの生成</summary>
        /// <param name="hBitmap">アンマネージビットマップオブジェクトのハンドル</param>
        /// <parrm name="text">テキスト文字列</parrm>
        /// <returns>ImageViewオブジェクト</returns>
        private ImageView CreateBitmapViewBox(IntPtr hBitmap, string text)
        {
            // ビットマップイメージの生成
            Image image = new Image();
            image.Source = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            // イメージビューの生成
            var view = new ImageView();
            view.xViewbox.Child = image;
            view.xTextBlock.Text = text;
            view.Visibility = Visibility.Visible;
            return view;
        }
        #endregion
    }
}
