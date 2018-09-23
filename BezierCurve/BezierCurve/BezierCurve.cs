using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BezierCurve
{
    public class BezierCurve
    {
        private float _elapsedTime;
        private float _time;
        private List<Vector2> _points;
        private float _t;
        private bool _finished;
        private bool _paused;

        public List<Vector2> PointsOnCurve { get; private set; }
        public Vector2 GetRecentPointOnCurve => PointsOnCurve.LastOrDefault();

        public BezierCurve(float timeInSeconds, List<Vector2> points)
        {
            if(timeInSeconds <= 0) throw new ArgumentException("Bezier Curves require a positive time greater than 0 to work", nameof(timeInSeconds));
            if(points.Count < 2) throw new ArgumentException("Bezier Curves require two or more points to work", nameof(points));

            _time = timeInSeconds;
            _points = points;

            Reset();
        }

        public void Reset()
        {
            _elapsedTime = 0;
            _t = 0;
            _finished = false;
            _paused = false;
            PointsOnCurve = new List<Vector2> { _points.FirstOrDefault() };
        }

        public void Pause()
        {
            _paused = !_paused;
        }

        public void Update(float deltaTime)
        {
            if (_finished || _paused) return;

            _elapsedTime += deltaTime;
            _t = _elapsedTime / _time;

            if (_t >= 1)
            {
                _finished = true;

                if(!PointsOnCurve.Last().Equals(_points.Last())) PointsOnCurve.Add(_points.Last());
            }
            else
            {
                PointsOnCurve.Add(GetPointOnCurve(_points));
            }
        }

        private Vector2 GetPointOnCurve(IReadOnlyCollection<Vector2> points)
        {
            /*

            Iterative Example
             
            List<Vector2> pointsToLerp = new List<Vector2>();
            List<Vector2> pointsSubset = points.ToList();

            do
            {
                pointsToLerp = pointsSubset.ToList();
                pointsSubset.Clear();

                for (var i = 1; i < pointsToLerp.Count; i++)
                {
                    pointsSubset.Add((1 - _t) * pointsToLerp[i-1] + _t * pointsToLerp[i]);
                }
            } while (pointsSubset.Count > 1);

            return pointsSubset.First();*/

            if (points.Count == 1) return points.First();

            var pa = GetPointOnCurve(points.Take(points.Count - 1).ToList());
            var pb = GetPointOnCurve(points.Skip(1).ToList());

            return (1 - _t) * pa + _t * pb;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D mainLinesPixel, List<Texture2D> sublinesPixels)
        {
            DrawMainLines(spriteBatch, mainLinesPixel);
            DrawSubLines(spriteBatch, new Stack<Texture2D>(sublinesPixels));
            DrawPoints(spriteBatch, mainLinesPixel);
        }

        private void DrawMainLines(SpriteBatch spriteBatch, Texture2D pixel)
        {
            for (int i = 1; i < _points.Count; i++)
            {
                var distance = Vector2.Distance(_points[i - 1], _points[i]);
                var angle = (float) Math.Atan2(_points[i].Y - _points[i - 1].Y, _points[i].X - _points[i - 1].X);

                spriteBatch.Draw(pixel, _points[i - 1], null, Color.White, angle, Vector2.Zero, new Vector2(distance, 1.0f), SpriteEffects.None, 0.0f);
            }
        }

        private void DrawSubLines(SpriteBatch spriteBatch, Stack<Texture2D> sublinesPixels)
        {
            if (_points.Count == 2) return;

            List<List<Vector2>> subpointList = new List<List<Vector2>>();

            do
            {
                subpointList.Add(GetSubPoints(subpointList.LastOrDefault() ?? _points));
            } while (subpointList.Last().Count > 2);

            Texture2D sublinePixel = null;
            foreach (var subpoints in subpointList)
            {
                sublinePixel = sublinesPixels.Count == 0 ? sublinePixel : sublinesPixels.Pop();
                for (int i = 1; i < subpoints.Count; i++)
                {
                    var distance = Vector2.Distance(subpoints[i - 1], subpoints[i]);
                    var angle = (float)Math.Atan2(subpoints[i].Y - subpoints[i - 1].Y, subpoints[i].X - subpoints[i - 1].X);

                    spriteBatch.Draw(sublinePixel, subpoints[i - 1], null, Color.White * 0.5f, angle, Vector2.Zero, new Vector2(distance, 1.0f), SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(sublinePixel, subpoints[i - 1], null, Color.White * 0.5f, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);
                    spriteBatch.Draw(sublinePixel, subpoints[i], null, Color.White * 0.5f, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);
                }
            }
        }

        private List<Vector2> GetSubPoints(List<Vector2> points)
        {
            var result = new List<Vector2>();

            for (int i = 1; i < points.Count; i++)
            {
                var p0 = points[i - 1];
                var p1 = points[i];

                result.Add((1 - _t) * p0 + _t * p1);
            }

            return result;
        }

        private void DrawPoints(SpriteBatch spriteBatch, Texture2D pixel)
        {
            foreach (var point in PointsOnCurve)
            {
                spriteBatch.Draw(pixel, point, null, Color.White * 0.05f, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0.0f);
            }
        }
    }
}
