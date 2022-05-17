using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace WordleSolver2 {
    public partial class MainWindow : Window {
        private readonly IBrush? yellowBrush  = new BrushConverter().ConvertFrom("#cec021") as Brush;
        private readonly IBrush? greenBrush  = new BrushConverter().ConvertFrom("#21ce43") as Brush;
        private int stage = 0;
        private string hypotheticalString = "*****";
        private List<char> invalidList = new List<char>();
        private List<YellowLetter> knownYellowLetters = new List<YellowLetter>();
        private readonly Dictionary<char, double> _rank;

        private readonly CheckBox _checkBox;
        
        private readonly TextBox _box0x0;
        private readonly TextBox _box0x1;
        private readonly TextBox _box0x2;
        private readonly TextBox _box0x3;
        private readonly TextBox _box0x4;
        private readonly TextBox _box1x0;
        private readonly TextBox _box1x1;
        private readonly TextBox _box1x2;
        private readonly TextBox _box1x3;
        private readonly TextBox _box1x4;
        private readonly TextBox _box2x0;
        private readonly TextBox _box2x1;
        private readonly TextBox _box2x2;
        private readonly TextBox _box2x3;
        private readonly TextBox _box2x4;
        private readonly TextBox _box3x0;
        private readonly TextBox _box3x1;
        private readonly TextBox _box3x2;
        private readonly TextBox _box3x3;
        private readonly TextBox _box3x4;
        private readonly TextBox _box4x0;
        private readonly TextBox _box4x1;
        private readonly TextBox _box4x2;
        private readonly TextBox _box4x3;
        private readonly TextBox _box4x4;
        private readonly TextBox _box5x0;
        private readonly TextBox _box5x1;
        private readonly TextBox _box5x2;
        private readonly TextBox _box5x3;
        private readonly TextBox _box5x4;
        
        public MainWindow() {
            InitializeComponent();

            _checkBox = this.FindControl<CheckBox>("CheckBox");

            _box0x0 = this.FindControl<TextBox>("Box0X0");
            _box0x1 = this.FindControl<TextBox>("Box0X1");
            _box0x2 = this.FindControl<TextBox>("Box0X2");
            _box0x3 = this.FindControl<TextBox>("Box0X3");
            _box0x4 = this.FindControl<TextBox>("Box0X4");
            
            _box1x0 = this.FindControl<TextBox>("Box1X0");
            _box1x1 = this.FindControl<TextBox>("Box1X1");
            _box1x2 = this.FindControl<TextBox>("Box1X2");
            _box1x3 = this.FindControl<TextBox>("Box1X3");
            _box1x4 = this.FindControl<TextBox>("Box1X4");
            
            _box2x0 = this.FindControl<TextBox>("Box2X0");
            _box2x1 = this.FindControl<TextBox>("Box2X1");
            _box2x2 = this.FindControl<TextBox>("Box2X2");
            _box2x3 = this.FindControl<TextBox>("Box2X3");
            _box2x4 = this.FindControl<TextBox>("Box2X4");
            
            _box3x0 = this.FindControl<TextBox>("Box3X0");
            _box3x1 = this.FindControl<TextBox>("Box3X1");
            _box3x2 = this.FindControl<TextBox>("Box3X2");
            _box3x3 = this.FindControl<TextBox>("Box3X3");
            _box3x4 = this.FindControl<TextBox>("Box3X4");
            
            _box4x0 = this.FindControl<TextBox>("Box4X0");
            _box4x1 = this.FindControl<TextBox>("Box4X1");
            _box4x2 = this.FindControl<TextBox>("Box4X2");
            _box4x3 = this.FindControl<TextBox>("Box4X3");
            _box4x4 = this.FindControl<TextBox>("Box4X4");
            
            _box5x0 = this.FindControl<TextBox>("Box5X0");
            _box5x1 = this.FindControl<TextBox>("Box5X1");
            _box5x2 = this.FindControl<TextBox>("Box5X2");
            _box5x3 = this.FindControl<TextBox>("Box5X3");
            _box5x4 = this.FindControl<TextBox>("Box5X4");

            _rank = new Dictionary<char, double>() {
                {'e', 11.1607},
                {'a', 8.4966},
                {'r', 7.5809},
                {'i', 7.5448},
                {'o', 7.1635},
                {'t', 6.9509},
                {'n', 6.6544},
                {'s', 5.7351},
                {'l', 5.4893},
                {'c', 4.5388},
                {'u', 3.6308},
                {'d', 3.3844},
                {'p', 3.1671},
                {'m', 3.0129},
                {'h', 3.0034},
                {'g', 2.4705},
                {'b', 2.0720},
                {'f', 1.8121},
                {'y', 1.7779},
                {'w', 1.2899},
                {'k', 1.1016},
                {'v', 1.0074},
                {'x', 0.2902},
                {'z', 0.2722},
                {'j', 0.1965},
                {'q', 0.1962}
            };
        }

        private void TextBox_DoubleTapped(object? sender, RoutedEventArgs e) {
            if (sender is TextBox item) {
                if (Equals(item.BorderBrush, greenBrush)) {
                    item.BorderBrush = yellowBrush;
                    return;
                }
                if (Equals(item.BorderBrush, yellowBrush)) {
                    item.BorderBrush = greenBrush;
                    return;
                }
                if (!Equals(item.BorderBrush, yellowBrush) && !Equals(item.BorderBrush, greenBrush)) {
                    item.BorderBrush = yellowBrush;
                }
            }
        }

        private void NarrowPossibilities(string[] dictionary) {
            var possibilities = new List<string>();
            foreach (var word in dictionary) {
                var validWord = true;

                foreach (var letter in invalidList) {
                    if (word.Contains(letter)) {
                        validWord = false;
                        break;
                    }
                }
                
                for (int i = 0; i < 5; i++) {
                    if (hypotheticalString[i] != '*') {
                        // If the letter doesn't match the green letters it's definitely not right
                        if (word[i] != hypotheticalString[i]) {
                            validWord = false;
                            break;
                        }
                    }

                    foreach (var letter in knownYellowLetters) {
                        if (!word.Contains(letter.Letter)) {
                            validWord = false;
                            break;
                        }
                        foreach (var pos in letter.Possibilities) {
                            if (word[i] == char.Parse(letter.Letter)) {
                                if (!letter.Possibilities[i]) {
                                    validWord = false;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (validWord) {
                    possibilities.Add(word);
                }
            }

            var newPossibilities = RankWords(possibilities);

            WordList.Text = "";
            foreach (var word in newPossibilities) {
                WordList.Text += "\n" + word;
            }
        }

        private List<string> RankWords(List<string> p) {
            var dict = new Dictionary<string, double>();
            foreach (var word in p) {
                double rank = 0.0;
                foreach (var letter in word) {
                    rank += _rank[letter];
                }

                dict.Add(word, rank);
            }

            var sortedDict = from entry in dict orderby entry.Value ascending select entry;

            return sortedDict.Select(pair => pair.Key).ToList();
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e) {
            string[] dictionary;
            if (_checkBox.IsChecked ?? false) {
                dictionary = File.ReadAllLines("duplicates.txt");
            }
            else {
                dictionary = File.ReadAllLines("dictionary.txt");
            }
            
            
            TextBox[] boxes = new TextBox[] { };
            TextBox[] newBoxes = new TextBox[] { };
            if (stage == 0) {
                boxes = new TextBox[] {
                    _box0x0,
                    _box0x1,
                    _box0x2,
                    _box0x3,
                    _box0x4,
                };
                newBoxes = new TextBox[] {
                    _box1x0,
                    _box1x1,
                    _box1x2,
                    _box1x3,
                    _box1x4,
                };
            }
            else if (stage == 1) {
                boxes = new TextBox[] {
                    _box1x0,
                    _box1x1,
                    _box1x2,
                    _box1x3,
                    _box1x4,
                };
                newBoxes = new TextBox[] {
                    _box2x0,
                    _box2x1,
                    _box2x2,
                    _box2x3,
                    _box2x4,
                };
            }
            else if (stage == 2) {
                boxes = new TextBox[] {
                    _box2x0,
                    _box2x1,
                    _box2x2,
                    _box2x3,
                    _box2x4,
                };
                newBoxes = new TextBox[] {
                    _box3x0,
                    _box3x1,
                    _box3x2,
                    _box3x3,
                    _box3x4,
                };
            }
            else if (stage == 3) {
                boxes = new TextBox[] {
                    _box3x0,
                    _box3x1,
                    _box3x2,
                    _box3x3,
                    _box3x4,
                };
                newBoxes = new TextBox[] {
                    _box4x0,
                    _box4x1,
                    _box4x2,
                    _box4x3,
                    _box4x4,
                };
            }
            else if (stage == 4) {
                boxes = new TextBox[] {
                    _box4x0,
                    _box4x1,
                    _box4x2,
                    _box4x3,
                    _box4x4,
                };
                newBoxes = new TextBox[] {
                    _box5x0,
                    _box5x1,
                    _box5x2,
                    _box5x3,
                    _box5x4,
                };
            }
            else if (stage == 5) {
                boxes = new TextBox[] {
                    _box5x0,
                    _box5x1,
                    _box5x2,
                    _box5x3,
                    _box5x4,
                };
            }

            for (int i = 0; i < 5; i++) {
                if (Equals(boxes[i].BorderBrush, greenBrush)) {
                    StringBuilder sb = new StringBuilder(hypotheticalString);
                    sb[i] = char.Parse(boxes[i].Text);
                    hypotheticalString = sb.ToString();
                    newBoxes[i].BorderBrush = greenBrush;
                    newBoxes[i].Text = boxes[i].Text;
                }
                else if (Equals(boxes[i].BorderBrush, yellowBrush)) {
                    bool foundLetter = false;
                    if (knownYellowLetters.Count != 0) {
                        foreach (var letter in knownYellowLetters) {
                            if (letter.Letter == boxes[i].Text) {
                                letter.Possibilities[i] = false;
                                foundLetter = true;
                            }
                        }
                    }

                    if (!foundLetter) {
                        var newLetter = new YellowLetter {
                            Letter = boxes[i].Text,
                            Possibilities = new bool[] {
                                true,
                                true,
                                true,
                                true,
                                true
                            }
                        };
                        newLetter.Possibilities[i] = false;
                        knownYellowLetters.Add(newLetter);
                    }
                }
                else {
                    invalidList.Add(char.Parse(boxes[i].Text));
                }
            }

            if (stage != 5) {
                foreach (var box in boxes) {
                    box.IsEnabled = false;
                }

                foreach (var box in newBoxes) {
                    box.IsEnabled = true;
                }
            }
            
            NarrowPossibilities(dictionary);
            if (sender != null) {
                stage++;
            }
            
        }

        private void CheckBox_Changed(object? sender, RoutedEventArgs e) {
            string[] dictionary;
            if (_checkBox.IsChecked ?? false) {
                dictionary = File.ReadAllLines("duplicates.txt");
            }
            else {
                dictionary = File.ReadAllLines("dictionary.txt");
            }
            NarrowPossibilities(dictionary);
        }
    }
}