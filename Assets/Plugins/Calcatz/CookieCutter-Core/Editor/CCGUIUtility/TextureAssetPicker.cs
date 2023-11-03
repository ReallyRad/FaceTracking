using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.CookieCutter {
    public class TextureAssetPicker : EditorWindow {
        
        public static void ShowPicker(IEnumerable<Sprite> _sprites, System.Action<Sprite> _onSelect, IEnumerable<string> _names = null) {
            ShowPicker(_sprites.Select(_sprite => _sprite == null? null : (UnityEngine.Object)_sprite), _object => {
                if (_onSelect != null) {
                    _onSelect.Invoke((Sprite)_object);
                }
            }, _names);
        }

        public static void ShowPicker(IEnumerable<Texture2D> _textures, System.Action<Texture2D> _onSelect, IEnumerable<string> _names = null) {
            ShowPicker(_textures.Select(_texture => _texture == null ? null : (UnityEngine.Object)_texture), _object => {
                if (_onSelect != null) {
                    _onSelect.Invoke((Texture2D)_object);
                }
            }, _names);
        }

        private static void ShowPicker(IEnumerable<UnityEngine.Object> _textures, System.Action<UnityEngine.Object> _onSelect, IEnumerable<string> _names = null) {
            TextureAssetPicker window = GetWindow<TextureAssetPicker>();
            window.titleContent.text = "Texture Picker";
            window.textures = _textures.ToList();
            window.row = Mathf.CeilToInt(window.textures.Count / (float)column);
            int usedRow = Mathf.Min(window.row, maxRow);
            float scrollerWidth = window.row > maxRow ? 13 : 0;
            window.minSize = new Vector2(column * squareSize - margin*column + margin + scrollerWidth, usedRow * squareSize - margin * 2 * usedRow + searchFieldHeight);
            window.maxSize = window.minSize;
            window.onSelect = _onSelect;
            if (_names != null) {
                window.names = _names.ToList();
            }
            else {
                window.names = _textures.Select(_texture => _texture == null? "None" : _texture.name).ToList();
            }
            window.filteredTextures = new List<Object>(window.textures);
            window.filteredNames = new List<string>(window.names);
            window.Show();
            window.RecreateGUI();
        }

        private static float searchFieldHeight = 16;
        private static float squareSize = 100;
        private static int column = 5;
        private static int margin = 4;
        private static int maxRow = 4;
        private int row;
        private List<UnityEngine.Object> textures;
        private List<UnityEngine.Object> filteredTextures;
        private List<string> names;
        private List<string> filteredNames;

        private VisualElement container;
        private System.Action<UnityEngine.Object> onSelect;

        private void CreateGUI() {
            RecreateGUI();
        }

        private void RecreateGUI() {
            VisualElement root = rootVisualElement;
            root.Clear();

            TextField searchField = new TextField() {
                style = {
                    flexGrow = 0,
                    marginLeft = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    marginRight = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    height = new StyleLength(new Length(searchFieldHeight, LengthUnit.Pixel))
                }
            };
            root.Add(searchField);
            searchField.Q("unity-text-input").Focus();

            searchField.RegisterValueChangedCallback(_event => {
                if (_event.newValue == null || _event.newValue == "") {
                    filteredTextures = new List<UnityEngine.Object>(textures);
                    filteredNames = new List<string>(names);
                }
                else {
                    filteredTextures.Clear();
                    filteredNames.Clear();
                    string lowerSearch = _event.newValue.ToLower();
                    for (int i = 0; i < names.Count; i++) {
                        if (names[i].ToLower().Contains(lowerSearch)) {
                            filteredTextures.Add(textures[i]);
                            filteredNames.Add(names[i]);
                        }
                    }
                }
                RecreateContents(root);
            });

            RecreateContents(root);
        }

        private void RecreateContents(VisualElement root) {
            if (container != null) {
                if (root.Contains(container)) {
                    root.Remove(container);
                    container.Clear();
                    container = null;
                }
            }
            if (filteredTextures == null || filteredTextures.Count == 0) return;
            int elementIndex = 0;
            if (row > maxRow) {
                container = CreateScrollView(root);
            }
            else {
                container = new VisualElement() {
                    style = {
                        position = Position.Absolute,
                        left = 0, right = 0,
                        top = searchFieldHeight, bottom = 0
                    }
                };
            }
            root.Add(container);
            for (int i = 0; i < row; i++) {
                VisualElement row = CreateRow(container);
                for (int j = 0; j < column; j++) {
                    CreateColumn(elementIndex, row);
                    elementIndex++;
                    if (elementIndex >= filteredTextures.Count) return;
                }
            }
        }

        private static VisualElement CreateRow(VisualElement root) {
            VisualElement row = new VisualElement() {
                style = {
                        flexDirection = FlexDirection.Row,
                        flexShrink = 0,
                        flexGrow = 0,
                        marginLeft = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                        height = new StyleLength(new Length(squareSize - margin*2, LengthUnit.Pixel))
                    }
            };
            root.Add(row);
            return row;
        }

        private VisualElement CreateScrollView(VisualElement root) {
            ScrollView scrollView = new ScrollView() {
                style = {
                    position = Position.Absolute,
                    left = 0, right = 0,
                    top = searchFieldHeight, bottom = 0
                }
            };
            VisualElement contentContainer = scrollView.Q("unity-content-container");
            contentContainer.style.height = new StyleLength(new Length(row * squareSize - row * margin * 2 + margin, LengthUnit.Pixel));
            root.Add(scrollView);
            return scrollView;
        }
        
        private void CreateColumn(int elementIndex, VisualElement row) {
            int cachedElementIndex = elementIndex;
            VisualElement column = new VisualElement() {
                style = {
                    width = new StyleLength(new Length(squareSize - margin*2, LengthUnit.Pixel)),
                    flexShrink = 0,
                    flexGrow = 0,
                    marginTop = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    marginRight = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    borderBottomLeftRadius = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    borderBottomRightRadius = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    borderTopLeftRadius = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    borderTopRightRadius = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    backgroundColor = new StyleColor(EditorGUIUtility.isProSkin? Color.black * 0.5f : Color.white * 0.5f)
                }
            };
            column.RegisterCallback<MouseEnterEvent>(_event => {
                column.style.backgroundColor = new StyleColor(Color.blue * 0.3f);
            });
            column.RegisterCallback<MouseLeaveEvent>(_event => {
                column.style.backgroundColor = new StyleColor(EditorGUIUtility.isProSkin ? Color.black * 0.5f : Color.white * 0.5f);
            });
            column.RegisterCallback<MouseDownEvent>(_event => {
                if (onSelect != null) {
                    onSelect.Invoke(filteredTextures[cachedElementIndex]);
                }
                Close();
            });
            row.Add(column);
            CreateColumnContent(elementIndex, column);
        }

        private void CreateColumnContent(int elementIndex, VisualElement column) {
            float textHeight = EditorGUIUtility.singleLineHeight;
            VisualElement image = new VisualElement() {
                style = {
                    position = Position.Absolute,
                    bottom = new StyleLength(new Length(margin + textHeight, LengthUnit.Pixel)),
                    top = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    left = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    right = new StyleLength(new Length(margin, LengthUnit.Pixel)),
#if UNITY_2022_2_OR_NEWER
                    backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center),
                    backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center),
                    backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat),
                    backgroundSize = new BackgroundSize(BackgroundSizeType.Contain)
#else
                    unityBackgroundScaleMode =ScaleMode.ScaleToFit
#endif
        }
            };
            if (filteredTextures[elementIndex] is Sprite sprite) {
                image.style.backgroundImage = new StyleBackground(Background.FromTexture2D(sprite.texture));
            } 
            else if (filteredTextures[elementIndex] is Texture2D texture2D) {
                image.style.backgroundImage = new StyleBackground(Background.FromTexture2D(texture2D));
            }

            Label textureName = new Label(filteredNames[elementIndex]) {
                style = {
                    position = Position.Absolute,
                    bottom = new StyleLength(new Length(0, LengthUnit.Pixel)),
                    left = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    right = new StyleLength(new Length(margin, LengthUnit.Pixel)),
                    height = new StyleLength(new Length(textHeight, LengthUnit.Pixel)),
                    overflow = Overflow.Hidden,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = new StyleLength(new Length(10, LengthUnit.Pixel))
                }
            };
            column.Add(image);
            column.Add(textureName);
        }

        private void OnGUI() {
            if (focusedWindow != this) {
                Close();
                return;
            }
        }
    }
}
