using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CocosSharp;
using BouncingGame.Entitities;

namespace BouncingGame
{
    
    public class GameLayer : CCLayerColor
    {
        List<Ball> Balls = new List<Ball>();

        float screenRight;
        float screenLeft;

        float screenTop;
        float screenBottom;

        CCLabel scoreLabel, livesLabel;
        CCSprite paddleSprite;

        // How much to modify the ball's y velocity per second:
        const float gravity = 140;

        int prevScore = 0;

        int score;
        int lives;

        public GameLayer() : base(CCColor4B.Black)
        {
            // create and initialize a Label
            livesLabel = new CCLabel("Lives: 5", "Arial", 40, CCLabelFormat.SystemFont);
            livesLabel.PositionX = 200;
            livesLabel.PositionY = 500;
            livesLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(livesLabel);

            // create and initialize a Label
            scoreLabel = new CCLabel("Score: 0", "Arial", 40, CCLabelFormat.SystemFont);
            scoreLabel.PositionX = 50;
            scoreLabel.PositionY = 500;
            scoreLabel.AnchorPoint = CCPoint.AnchorUpperLeft;
            AddChild(scoreLabel);

            //init paddle
            paddleSprite = new CCSprite("paddle");

            //set starting position of paddle
            paddleSprite.PositionX = 100;
            paddleSprite.PositionY = 50;
            AddChild(paddleSprite);

            AddBall();
            Schedule(RunGame);

        }

        void RunGame(float frameTimeInSeconds)
        {
            foreach(var ball in Balls.ToList())
            {
                ball.Activity(frameTimeInSeconds, gravity, paddleSprite);
                if(ball.DidBallHitPaddle(paddleSprite))
                {
                    score++;
                    scoreLabel.Text = "Score: " + score;
                }
                ProcessBallHittingWall(ball);
                ball.DidBallsCollide(Balls);

                if(score % 5 == 0 && score != prevScore)
                {
                    prevScore = score;
                    AddBall();
                }
            }
 
        }

        private void Reset()
        {

        }

        private void ProcessBallHittingFloor(Ball ballSprite)
        {
            float ballBottom = ballSprite.BoundingBoxTransformedToWorld.MinY;
            screenBottom = VisibleBoundsWorldspace.MinY;

            if(ballBottom < screenBottom & ballSprite.YVelocity < 0)
            {
                if (Balls.Count > 1)
                {
                    Balls.Remove(ballSprite);
                    RemoveChild(ballSprite);
                }
                else
                {
                    ballSprite.YVelocity *= -1;
                }
            }
        }

        /// <summary>
        /// Check if the ball hit the ground or ceiling and bounce off it
        /// </summary>
        private void ProcessBallHittingCeiling(Ball ballSprite)
        {
            float ballTop = ballSprite.BoundingBoxTransformedToWorld.MaxY;
            

            screenTop = VisibleBoundsWorldspace.MaxY;


            bool shouldReflectYVelocity = (ballTop > screenTop & ballSprite.YVelocity > 0);
                

            if (shouldReflectYVelocity)
            {
                ballSprite.YVelocity *= -1;
            }
        }

        /// <summary>
        /// Check the collision state of the ball hitting the wall and ensure that it bounces off
        /// </summary>
        private void ProcessBallHittingWall(Ball ballSprite)
        {
            //Get ball position
            ProcessBallHittingSides(ballSprite);
            ProcessBallHittingCeiling(ballSprite);
            ProcessBallHittingFloor(ballSprite);
        }

        private void ProcessBallHittingSides(Ball ballSprite)
        {
            float ballRight = ballSprite.BoundingBoxTransformedToWorld.MaxX;
            float ballLeft = ballSprite.BoundingBoxTransformedToWorld.MinX;

            screenRight = VisibleBoundsWorldspace.MaxX;
            screenLeft = VisibleBoundsWorldspace.MinX;

            //ensure ball bounces off of wall
            bool shouldReflectXVelocity =
                (ballRight > screenRight & ballSprite.XVelocity > 0) ||
                (ballLeft < screenLeft & ballSprite.XVelocity < 0);

            if (shouldReflectXVelocity)
            {
                ballSprite.XVelocity *= -1;
            }
        }

        protected override void AddedToScene()
        {

            base.AddedToScene();

            // Use the bounds to layout the positioning of our drawable assets
            var bounds = VisibleBoundsWorldspace;

            // Register for touch events
            var touchListener = new CCEventListenerTouchAllAtOnce();
            touchListener.OnTouchesEnded = OnTouchesEnded;

            touchListener.OnTouchesMoved = HandleTouchesMoved;

            var tiltListener = new CCEventListenerAccelerometer();
            tiltListener.IsEnabled = true;
            tiltListener.OnAccelerate = MovePaddle;
            AddEventListener(touchListener, this);
            AddEventListener(tiltListener, this);
        }

        void AddBall()
        {

            screenRight = VisibleBoundsWorldspace.MaxX;
            screenLeft = VisibleBoundsWorldspace.MinX;

            var xPos = CCRandom.GetRandomFloat(screenLeft + 100, screenRight - 100);
            var newBall = new Ball(xPos);
            Balls.Add(newBall);
            AddChild(newBall);

        }

        void HandleTouchesMoved(System.Collections.Generic.List<CCTouch> touches, CCEvent touchEvent)
        {

            // we only care about the first touch:
            bool stopPaddle = (paddleSprite.BoundingBoxTransformedToWorld.MaxX > screenRight) ||
              (paddleSprite.BoundingBoxTransformedToWorld.MinX < screenLeft);

            

            if (stopPaddle)
            {
                if ((paddleSprite.BoundingBoxTransformedToWorld.MaxX > screenRight))
                {
                    paddleSprite.PositionX -= 10;
                }

                if ((paddleSprite.BoundingBoxTransformedToWorld.MinX < screenLeft))
                {
                    paddleSprite.PositionX += 10;
                }
            }
            else
            {
                var locationOnScreen = touches[0].Location;
                paddleSprite.PositionX = locationOnScreen.X;
            }

        }

        /// <summary>
        /// ToDo: Organise to tap into accelerometer to move paddle
        /// </summary>
        /// <param name="action"></param>
        void MovePaddle(CCEventAccelerate action)
        {
           
        }


        void OnTouchesEnded(List<CCTouch> touches, CCEvent touchEvent)
        {
            if (touches.Count > 0)
            {
                // Perform touch handling here
            }
        }
    }
}

