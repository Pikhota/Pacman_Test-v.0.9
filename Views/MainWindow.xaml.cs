﻿using System;
using System.Threading;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Pacman
{
	public delegate Rect GetRectDelegate(Image img);

	public delegate void SetVisibilityStatusDelegate(Image img);
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const double Speed = 0;
		private const double Step = 5;
		public Rect Pacman;
		public TranslateTransform Trans = new TranslateTransform();
		public DoubleAnimation AnimationX = new DoubleAnimation();
		public DoubleAnimation AnimationY = new DoubleAnimation();
		public SideCollision SideCollision;
		public int Life = 3;
		public bool exitOpen = false;

		public MainWindow()
		{
			InitializeComponent();
			KeyDown += Moving;
			Pacman = new Rect(0, 0, ImgPacman.Width, ImgPacman.Height);
			ImgPacman.RenderTransform = Trans;
		}

		#region Движение пакмена по событию (нажатие на клавиши - стрелочки)
		private void Moving(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Right:
					CurrentLocation(e);
					if (Pacman.Right < Win.Width)
					{
						if (IsCollisionGhost())
							break;

						if (IsFindTreasure())
						{
							SetCollapsedImage(Treasure);
							SetVisibleImage(Exit);
							exitOpen = true;
						}

						if (exitOpen && IsGetExit())
						{
							SetVisibleImage(Victory);
						}

						if (IsCollisionWall() && SideCollision.Equals(SideCollision.Right))
						{
							while (IsCollisionWall())
							{
								AnimationX = new DoubleAnimation(Pacman.X--, TimeSpan.FromMilliseconds(Speed));
								Trans.BeginAnimation(TranslateTransform.XProperty, AnimationX);
								AnimateY();
							}
						}
						else
						{
							SideCollision = SideCollision.Right;
							AnimationX = new DoubleAnimation(Pacman.X + Step, TimeSpan.FromMilliseconds(Speed));
							Trans.BeginAnimation(TranslateTransform.XProperty, AnimationX);
							AnimateY();
						}
					}
					break;
				case Key.Left:
					CurrentLocation(e);
					if (Pacman.Left > 0)
					{
						if (IsCollisionGhost())
							break;

						if (IsFindTreasure())
						{
							SetCollapsedImage(Treasure);
							SetVisibleImage(Exit);
							exitOpen = true;
						}

						if (exitOpen && IsGetExit())
						{
							SetVisibleImage(Victory);
						}

						if (IsCollisionWall() && SideCollision.Equals(SideCollision.Left))
						{
							while (IsCollisionWall())
							{
								AnimationX = new DoubleAnimation(Pacman.X++, TimeSpan.FromMilliseconds(Speed));
								Trans.BeginAnimation(TranslateTransform.XProperty, AnimationX);
								AnimateY();
							}
						}
						else
						{
							SideCollision = SideCollision.Left;
							AnimationX = new DoubleAnimation(Pacman.X - Step, TimeSpan.FromMilliseconds(Speed));
							Trans.BeginAnimation(TranslateTransform.XProperty, AnimationX);
							AnimateY();
						}
					}
					break;
				case Key.Up:
					CurrentLocation(e);
					if (Pacman.Top > 0)
					{
						if (IsCollisionGhost())
							break;

						if (IsFindTreasure())
						{
							SetCollapsedImage(Treasure);
							SetVisibleImage(Exit);
							exitOpen = true;
						}

						if (exitOpen && IsGetExit())
						{
							SetVisibleImage(Victory);
						}

						if (IsCollisionWall() && SideCollision.Equals(SideCollision.Top))
						{
							while (IsCollisionWall())
							{
								AnimateX();
								AnimationY = new DoubleAnimation(Pacman.Y++, TimeSpan.FromMilliseconds(Speed));
								Trans.BeginAnimation(TranslateTransform.YProperty, AnimationY);
							}
						}
						else
						{
							SideCollision = SideCollision.Top;
							AnimateX();
							AnimationY = new DoubleAnimation(Pacman.Y - Step, TimeSpan.FromMilliseconds(Speed));
							Trans.BeginAnimation(TranslateTransform.YProperty, AnimationY);
						}

					}
					break;
				case Key.Down:
					CurrentLocation(e);
					if (Pacman.Bottom < Win.Height - Step * 2)
					{
						if (IsCollisionGhost())
							break;

						if(IsFindTreasure())
						{
							SetCollapsedImage(Treasure);
							SetVisibleImage(Exit);
							exitOpen = true;
						}

						if (exitOpen && IsGetExit())
						{
							SetVisibleImage(Victory);
						}

						if (IsCollisionWall() && SideCollision.Equals(SideCollision.Bottom))
						{
							while (IsCollisionWall())
							{
								AnimationY = new DoubleAnimation(Pacman.Y--, TimeSpan.FromMilliseconds(Speed));
								Trans.BeginAnimation(TranslateTransform.YProperty, AnimationY);
							}
						}
						else
						{
							SideCollision = SideCollision.Bottom;
							AnimateX();
							AnimationY = new DoubleAnimation(Pacman.Y + Step, TimeSpan.FromMilliseconds(Speed));
							Trans.BeginAnimation(TranslateTransform.YProperty, AnimationY);
						}
					}
					break;
			}
		}
		#endregion

		#region Проверки на столкновение со стеной и привидениями
		private bool IsCollisionWall()
		{
			var flag = false;
			var walls = new[]{ WallA1, WallA2, WallA3, WallA4,WallA5,WallA6,WallA7,WallA8,WallC1,WallC2,WallC3
						, WallM1,WallM2,WallM3,WallM4,WallM5,WallN1,WallN2,WallN3,WallP1,WallP2,WallP3,WallP4};
			foreach (var wall in walls)
			{
				var rectWall = GetRect(wall);
				if (Pacman.IntersectsWith(rectWall))
				{
					flag = true;
				}
			}
			return flag;
		}

		private bool IsCollisionGhost()
		{
			var flag = false;
			var ghosts = new[] { ImgGhostWhite, ImgGhostRed, ImgGhostYellow, ImgGhostViolet };
			foreach (var ghost in ghosts)
			{
				var rectGhost = GetRect(ghost);
				if (Pacman.IntersectsWith(rectGhost))
				{
					CatchedByGhost();

					flag = true;
				}
			}
			return flag;
		}

		private bool IsFindTreasure()
		{
		  Rect treasure = GetRect(Treasure);

			if (Pacman.IntersectsWith(treasure))
			{
				return true;
			}

			return false;
		}

		private bool IsGetExit()
		{
			Rect exit = GetRect(Exit);

			if (Pacman.IntersectsWith(exit))
			{
				return true;
			}

			return false;
		}
		#endregion

		#region Вспомогающие методы (уменьшение дублированого кода и т.п.)

		private void AnimateX()
		{
			AnimationX = new DoubleAnimation(Pacman.X, TimeSpan.FromMilliseconds(Speed));
			Trans.BeginAnimation(TranslateTransform.XProperty, AnimationX);
		}

		private void AnimateY()
		{
			AnimationY = new DoubleAnimation(Pacman.Y, TimeSpan.FromMilliseconds(Speed));
			Trans.BeginAnimation(TranslateTransform.YProperty, AnimationY);
		}

		private Rect GetRect(FrameworkElement element)
		{
			var transElement = element.TransformToVisual(this);
			return transElement.TransformBounds(new Rect(new Size(element.ActualWidth, element.ActualHeight)));
		}


		private void CurrentLocation(KeyEventArgs e)
		{
			if (Key.Right == e.Key || Key.Left == e.Key)
				Pacman.X = Trans.X;
			if (Key.Up == e.Key || Key.Down == e.Key)
				Pacman.Y = Trans.Y;
		}
		#endregion

		private void CatchedByGhost()
		{
			SetVisibleImage(GameOver);
		}

		private void SetCollapsedImage(Image img)
		{
			if (img.Visibility == Visibility.Visible || img.Visibility == Visibility.Hidden)
				img.Visibility = Visibility.Collapsed;
		}

		private void SetVisibleImage(Image img)
		{
			if (img.Visibility == Visibility.Hidden)
				img.Visibility = Visibility.Visible;
		}
	}
}

