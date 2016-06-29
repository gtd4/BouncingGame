using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncingGame.Entitities
{
    public class Ball : CCSprite
    {

        public Ball ():base("ball")
        {


        }

        public Ball(float xPosition):this()
        {

            this.Position = new CCPoint(xPosition, 500);
        }
        public float XVelocity { get; set; }
        public float YVelocity { get; set; }

        public void Activity(float frameTimeInSeconds, float gravity, CCSprite paddleSprite)
        {
            YVelocity += frameTimeInSeconds * -gravity;

            this.PositionX += XVelocity * frameTimeInSeconds;
            this.PositionY += YVelocity * frameTimeInSeconds;


        }

        /// <summary>
        /// Check the collision state between the ball and the paddle and ensure that it bounces off
        /// </summary>
        public bool DidBallHitPaddle(CCSprite paddleSprite)
        {
            bool doesCollide = this.BoundingBoxTransformedToParent.IntersectsRect(paddleSprite.BoundingBoxTransformedToParent);

            bool ballFalling = YVelocity < 0;

            if (doesCollide && ballFalling)
            {
                //inverts velocity
                YVelocity *= -1;

                const float minXVelocity = -300;
                const float maxXVelocity = 300;
                XVelocity = CCRandom.GetRandomFloat(minXVelocity, maxXVelocity);

                
            }

            return doesCollide && ballFalling;
        }

        /// <summary>
        /// Check if balls collide and rebound them off of each other
        /// </summary>
        /// <param name="balls"></param>
        public void DidBallsCollide(List<Ball> balls)
        {
            foreach(var ball in balls)
            {
                if(ball == this)
                {
                    continue;
                }

                bool doesCollide = this.BoundingBoxTransformedToWorld.IntersectsRect(ball.BoundingBoxTransformedToWorld);

                if(doesCollide)
                {
                    XVelocity *= -1;
                }
            }
        }

       
    }
}
