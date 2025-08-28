using BluetoothLE_App.Enums;
using BluetoothLE_App.Helpers;
using BluetoothLE_App.Models;
using BluetoothLE_App.Views.Popups;
using Shiny.BluetoothLE;
using Shiny.BluetoothLE.Hosting;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IPeripheral = Shiny.BluetoothLE.IPeripheral;
using static BluetoothLE_App.Helpers.Constants.Texts;
namespace BluetoothLE_App.ViewModel
{
    internal class GameViewModel : BaseHostClientViewModel
    {
        #region Constants

        private const int StrokeSize = 15;
        private const int DimensionSize = 3;

        #endregion Constants

        #region Fields

        private readonly SKPaint _defaultPaint;
        private readonly string _characteristicUuid;
        private TicTacToeField? _currentField;
        private TicTacToeState _winner;
        private WinnerPositionState _position;
        private bool _isMyTurn;
        private int _count;
        public bool NotRedraw;
        private BleCharacteristicInfo? _characteristic;
        public IPeripheral? Peripheral;
        private BusyPopup _busyPopup;
        private bool _isEnd;
        private GameOverPopup? _gameOverPopup;

        #endregion Fields

        #region Properties

        #region IsRetry Property

        private bool? _isMyRetry;

        private bool? IsMyRetry
        {
            get => _isMyRetry;
            set
            {
                _isMyRetry = value;
                if (value == null) return;
                if (IsOpponentRetry == true && value is true)
                {
                    Task.Run(Reset);
                }
                else if (value == false)
                {
                    MainThread.InvokeOnMainThreadAsync(GoToMainMenuAsync);
                }
            }
        }

        private bool? _isOpponentRetry;

        private bool? IsOpponentRetry
        {
            get => _isOpponentRetry;
            set
            {
                _isOpponentRetry = value;
                if (value == null) return;
                if (IsMyRetry == true && value is true)
                {
                    Task.Run(Reset);
                }
                else if (value == false && IsMyRetry == null)
                {
                    MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        _gameOverPopup?.SetMessage(OpponentLeft);
                        _gameOverPopup?.SetIsEnableRetry(false);
                    });
                }
                else if (value == false && IsMyRetry == true)
                {
                    _gameOverPopup = null;

                    Task.Run(() => GameOver(OpponentLeft, false));
                }
            }
        }

        #endregion IsRetry Property

        #region WinnerLineCanvas Property

        public SKCanvasView? WinnerLineCanvas { get; set; }

        #endregion WinnerLineCanvas Property

        #region Fields Property

        public ObservableCollection<TicTacToeField> Fields { get; set; } = new();

        #endregion Fields Property

        #region CanvasTapCommand Property

        public ICommand CanvasTapCommand { get; set; }

        #endregion CanvasTapCommand Property

        #region IsConnected Property

        private bool _isConnected;

        public bool IsConnected
        {
            get => !_isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
            }
        }

        #endregion IsConnected Property

        #region CurrentMove Property

        private TicTacToeState _currentMove;

        public TicTacToeState CurrentMove
        {
            get => _currentMove;
            set
            {
                _currentMove = value;
                OnPropertyChanged();
            }
        }

        #endregion CurrentMove Property

        #region YourState Property

        private TicTacToeState _yourState;

        public TicTacToeState YourState
        {
            get => _yourState;

            set
            {
                _yourState = value;
                OnPropertyChanged();
            }
        }

        #endregion YourState Property

        #region Turn Property

        private int _turn;

        public int Turn
        {
            get => _turn;
            set
            {
                _turn = value;
                CurrentMove = value % 2 != 0 ? TicTacToeState.Cross : TicTacToeState.Zero;

                if (_turn >= 6)
                {
                    _winner = CheckWin();

                    if (value >= 10 && _winner == TicTacToeState.None)
                    {
                        Task.Run(() => GameOver(Draw));
                        _isEnd = true;
                    }
                    else if (_winner != TicTacToeState.None)
                    {
                        Task.Run(() => GameOver($"Winner {_winner}"));
                        WinnerLineCanvas?.InvalidateSurface();
                        _isEnd = true;
                    }
                }

                OnPropertyChanged();
            }
        }

        #endregion Turn Property

        #region Height Property

        private double _height;

        public double Height
        {
            get => _height;
            set
            {
                _height = value * DeviceDisplay.MainDisplayInfo.Density;
                OnPropertyChanged();
                for (var i = 0; i < DimensionSize; i++)
                {
                    for (var j = 0; j < DimensionSize; j++)
                    {
                        var canvas = new SKCanvasView() { WidthRequest = value, HeightRequest = value };
                        Fields.Add(new TicTacToeField
                        {
                            X = i,
                            Y = j,
                            Canvas = canvas
                        });
                        canvas.PaintSurface += CanvasPaintSurface;
                    }
                }
            }
        }

        #endregion

        #region OpponentIdentifier Property

        private string? _opponentIdentifier;

        public string OpponentIdentifier
        {
            get => _opponentIdentifier ?? string.Empty;
            set
            {
                _opponentIdentifier = value;
                OnPropertyChanged();
            }
        }

        #endregion OpponentIdentifier Property

        #region YoursCoordinates Property

        private Vector2 _yoursCoordinates;

        public Vector2 YoursCoordinates
        {
            get => _yoursCoordinates;

            set
            {
                if (!_isConnected || !_isMyTurn) return;
                _isMyTurn = false;
                _yoursCoordinates = value;
                OnPropertyChanged();

                _currentField?.Canvas?.InvalidateSurface();


                var data = Encoding.UTF8.GetBytes($"{_yoursCoordinates.X}:{_yoursCoordinates.Y}");

                if (_characteristic != null && Peripheral != null)
                {
                    try
                    {
                        Task.Run(() => WriteDataAsync(data));
                    }
                    catch (Exception e)
                    {
                        MainThread.InvokeOnMainThreadAsync(() => Shell.Current.DisplayAlert("error", e.Message, "ok"));
                    }
                }

                Turn++;
            }
        }

        #endregion YoursCoordinates Property

        #region TheirCoordinates Property

        private Vector2 TheirCoordinates
        {
            set
            {
                _currentField = Fields[(int)(value.X + value.Y * DimensionSize)];
                _currentField.State = _yourState == TicTacToeState.Cross ? TicTacToeState.Zero : TicTacToeState.Cross;
                _currentField?.Canvas?.InvalidateSurface();
                _isMyTurn = true;
                Turn++;
            }
        }

        #endregion TheirCoordinates Property

        #endregion Properties

        public GameViewModel(string opponentIdentifier, TicTacToeState yourState, string characteristicUuid)
        {
            IsOpponentRetry = null;
            IsMyRetry = null;

            _isEnd = false;
            _yourState = yourState;
            _count = 0;
            _isMyTurn = yourState == TicTacToeState.Cross;
            _characteristicUuid = characteristicUuid;
            _busyPopup = new BusyPopup();
            _currentField = null;
            IsConnected = false;
            OpponentIdentifier = opponentIdentifier;
            Turn = 1;
            MainThread.InvokeOnMainThreadAsync(() => _busyPopup.ShowAsync());

            CanvasTapCommand = new Command<TicTacToeField>(OnCanvasTap);

            _defaultPaint = new SKPaint
            {
                Color = new SKColor(255, 0, 0),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = StrokeSize
            };

            Task.Run(() => StartServer(DataTransferServiceUuid));

            StartScan();
        }

        private void OnCanvasTap(TicTacToeField field)
        {
            if (field.State != TicTacToeState.None || !_isMyTurn || IsConnected) return;
            field.State = _yourState;
            _currentField = field;
            YoursCoordinates = new Vector2(field.Y, field.X);
        }

        private void CanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            var canvas = surface.Canvas;

            if (NotRedraw)
            {
                _count++;
                if (_count == 9)
                {
                    NotRedraw = false;
                    _count = 0;
                }

                return;
            }

            if (_currentField == null) return;

            canvas.Clear();

            switch (_currentField.State)
            {
                case TicTacToeState.None:
                    break;

                case TicTacToeState.Cross:
                    canvas.DrawLine(0, 0, (float)Height, (float)Height, _defaultPaint);
                    canvas.DrawLine(0, (float)Height, (float)Height, 0, _defaultPaint);
                    break;

                case TicTacToeState.Zero:
                    canvas.DrawCircle((float)Height / 2, (float)Height / 2, (float)Height / 2 - StrokeSize, _defaultPaint);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void WinnerLineCanvasPaintSurface(object? sender, SKPaintSurfaceEventArgs args)
        {
            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            switch (_position)
            {
                case WinnerPositionState.None:
                    break;
                case WinnerPositionState.FirstRow:
                    canvas.DrawLine(0, (float)Height / 2, (float)Height * 3, (float)Height / 2, _defaultPaint);
                    break;
                case WinnerPositionState.SecondRow:
                    canvas.DrawLine(0, (float)Height * 1.5f, (float)Height * 3, (float)Height * 1.5f, _defaultPaint);
                    break;
                case WinnerPositionState.ThirdRow:
                    canvas.DrawLine(0, (float)Height * 2.5f, (float)Height * 3, (float)Height * 2.5f, _defaultPaint);
                    break;
                case WinnerPositionState.FirstColumn:
                    canvas.DrawLine((float)Height / 2, 0, (float)Height / 2, (float)Height * 3, _defaultPaint);
                    break;
                case WinnerPositionState.SecondColumn:
                    canvas.DrawLine((float)Height * 1.5f, 0, (float)Height * 1.5f, (float)Height * 3, _defaultPaint);
                    break;
                case WinnerPositionState.ThirdColumn:
                    canvas.DrawLine((float)Height * 2.5f, 0, (float)Height * 2.5f, (float)Height * 3, _defaultPaint);
                    break;
                case WinnerPositionState.UpToDownDiagonal:
                    canvas.DrawLine(0, 0, (float)Height * 3, (float)Height * 3, _defaultPaint);
                    break;
                case WinnerPositionState.DownToUpDiagonal:
                    canvas.DrawLine(0, (float)Height * 3, (float)Height * 3, 0, _defaultPaint);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void StartScan()
        {
            Task.Run(() =>
            {
                Scan(OnNext);
            });
        }

        private async void OnNext(IList<ScanResult> results)
        {
#if ANDROID
            var buf = results.FirstOrDefault(x => x.Peripheral.Name == OpponentIdentifier)
                ?.Peripheral;
#elif IOS
        var buf = results.FirstOrDefault(x => x.Peripheral.Uuid == OpponentIdentifier)
            ?.Peripheral;
#endif
            if (buf != null && Peripheral == null)
            {
                await SetPeripheral(buf);
            }
        }

        protected override void BuildService(IGattServiceBuilder serviceBuilder)
        {
            serviceBuilder.AddCharacteristic(
                _characteristicUuid,
                cb =>
                {
                    cb.SetWrite(request =>
                    {
                        if (_isEnd)
                        {
                            IsOpponentRetry = bool.Parse(Encoding.UTF8.GetString(request.Data, 0, request.Data.Length));
                            return Task.FromResult(GattState.Success);
                        }

                        var textCoordinates = Encoding.UTF8.GetString(request.Data, 0, request.Data.Length).Split(':');
                        TheirCoordinates = new Vector2(float.Parse(textCoordinates[0]), float.Parse(textCoordinates[1]));
                        return Task.FromResult(GattState.Success);
                    });
                }
            );
        }

        public override void StopScan()
        {
            if (ScanSub == null) return;
            base.StopScan();
        }

        private async Task SetPeripheral(Shiny.BluetoothLE.IPeripheral? peripheral)
        {
            if (peripheral == null) return;
            StopScan();
            Peripheral = peripheral;
            _characteristic = await BleHelper.GetCharacteristic(peripheral, DataTransferServiceUuid, _characteristicUuid);
            if (_characteristic == null)
            {
                Peripheral = null;
                StartScan();
            }
            else
            {
                IsConnected = true;
                await MainThread.InvokeOnMainThreadAsync(() => _busyPopup.Hide());
            }
        }

        private TicTacToeState CheckWin()
        {
            var result = TicTacToeState.None;

            #region Horzontal Winning Condtion

            //Winning Condition For First Row
            if (Fields[0].State != TicTacToeState.None && Fields[0].State == Fields[1].State &&
                Fields[1].State == Fields[2].State)
            {
                result = Fields[0].State;
                _position = WinnerPositionState.FirstRow;
            }
            //Winning Condition For Second Row
            else if (Fields[3].State != TicTacToeState.None && Fields[3].State == Fields[4].State &&
                     Fields[4].State == Fields[5].State)
            {
                result = Fields[3].State;
                _position = WinnerPositionState.SecondRow;
            }
            //Winning Condition For Third Row
            else if (Fields[6].State != TicTacToeState.None && Fields[6].State == Fields[7].State &&
                     Fields[7].State == Fields[8].State)
            {
                result = Fields[6].State;
                _position = WinnerPositionState.ThirdRow;
            }

            #endregion

            #region vertical Winning Condtion

            //Winning Condition For First Column
            else if (Fields[0].State != TicTacToeState.None && Fields[0].State == Fields[3].State &&
                     Fields[3].State == Fields[6].State)
            {
                result = Fields[0].State;
                _position = WinnerPositionState.FirstColumn;
            }
            //Winning Condition For Second Column
            else if (Fields[1].State != TicTacToeState.None && Fields[1].State == Fields[4].State &&
                     Fields[4].State == Fields[7].State)
            {
                result = Fields[1].State;
                _position = WinnerPositionState.SecondColumn;
            }
            //Winning Condition For Third Column
            else if (Fields[2].State != TicTacToeState.None && Fields[2].State == Fields[5].State &&
                     Fields[5].State == Fields[8].State)
            {
                result = Fields[2].State;
                _position = WinnerPositionState.ThirdColumn;
            }

            #endregion

            #region Diagonal Winning Condition

            else if (Fields[0].State != TicTacToeState.None && Fields[0].State == Fields[4].State &&
                     Fields[4].State == Fields[8].State)
            {
                result = Fields[4].State;
                _position = WinnerPositionState.UpToDownDiagonal;
            }
            else if (Fields[2].State != TicTacToeState.None && Fields[2].State == Fields[4].State &&
                     Fields[4].State == Fields[6].State)
            {
                result = Fields[4].State;
                _position = WinnerPositionState.DownToUpDiagonal;
            }

            #endregion

            return result;
        }

        private async Task Reset()
        {
            _isEnd = false;
            _winner = TicTacToeState.None;
            _position = WinnerPositionState.None;
            if (_currentField != null) _currentField.State = TicTacToeState.None;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                YourState = _yourState == TicTacToeState.Zero ? TicTacToeState.Cross : TicTacToeState.Zero;
                Turn = 1;
                IsOpponentRetry = null;
                IsMyRetry = null;
            });


            _isMyTurn = _yourState == TicTacToeState.Cross;

            foreach (var field in Fields)
            {
                field.State = TicTacToeState.None;
                if (field.Canvas == null) break;
                await MainThread.InvokeOnMainThreadAsync(() =>
                    field.Canvas.InvalidateSurface());
            }

            if (WinnerLineCanvas == null) return;

            await MainThread.InvokeOnMainThreadAsync(() =>
               WinnerLineCanvas.InvalidateSurface());
            await MainThread.InvokeOnMainThreadAsync(() => _busyPopup.Hide());
        }

        private async Task GameOver(string winner, bool isEnableRetry = true)
        {
            var result = await MainThread.InvokeOnMainThreadAsync(() =>
            {
                _gameOverPopup = new GameOverPopup(winner);
                _gameOverPopup.SetIsEnableRetry(isEnableRetry);
                return _gameOverPopup.ShowAsync();
            });
            byte[] data;

            switch (result?.Status)
            {
                case DialogReturnStatuses.Positive:

                    data = Encoding.UTF8.GetBytes(bool.TrueString);
                    await WriteDataAsync(data);
                    IsMyRetry = true;
                    MainThread.InvokeOnMainThreadAsync(() => _busyPopup.ShowAsync());
                    break;

                case DialogReturnStatuses.Negative:
                    IsMyRetry = false;
                    data = Encoding.UTF8.GetBytes(bool.FalseString);
                    await WriteDataAsync(data);
                    await MainThread.InvokeOnMainThreadAsync(() => _busyPopup.Hide());
                    break;
            }
        }

        private async Task GoToMainMenuAsync()
        {
            await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("../.."));
        }

        private async Task WriteDataAsync(byte[] data)
        {
            try
            {
                await Peripheral!.WriteCharacteristicAsync(_characteristic!, data);
            }
            catch (Exception e)
            {
                IsOpponentRetry = false;
                Console.WriteLine(e);
            }
        }
    }
}