using Microsoft.Maui.Controls.Shapes;

namespace ArcSegmentRepro;

public partial class ArcView : ContentView
{
	public ArcView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty ViewRadiusProperty = BindableProperty.Create(
            nameof(ViewRadius),
            typeof(double),
            typeof(ArcView),
            defaultValue: 0.0);
    public double ViewRadius 
    {
        get { return (double)GetValue(ViewRadiusProperty); }
        set { SetValue(ViewRadiusProperty, value); }
    }
    private double ArcRadius { get => ViewRadius - StrokeThickness / 2.0d; }
    private double StrokeThickness { get => ViewRadius / 15.0d; }
    private double GoalRadius { get => ViewRadius / 28.0d; }

    private const int ANIMATION_LENGTH = 1000;

    private const double Max = 100;
    private const double Min = 0;

    private double _currentVal;

    public double CurrentVal
    {
        get => _currentVal;
        set
        {
            if (_currentVal != value)
            {
                _currentVal = value;
                OnPropertyChanged(nameof(CurrentVal));
            }
        }
    }

    public List<Point> StartPoints { get; set; } = new List<Point>();

    protected override void OnSizeAllocated(double width, double height)
    {
        if (width <= 0 || height <= 0) base.OnSizeAllocated(width, height);
        else
        {
            CoverPath.StrokeThickness = StrokeThickness;

            base.OnSizeAllocated(width, height);
            CreateArcAndLineSegment(CurrentVal, CurrentVal);
        }
    }


    public void CreateArcAndLineSegment(double oldValue, double currentValue)
    {
        CoverPath.CancelAnimations();
        if(AnimateCheckBox.IsChecked)
        {
            var animation = new Animation(v => CoverPath.Data = GetArcPath(v), oldValue, currentValue);
            CoverPath.Animate("coveranim", animation, length: ANIMATION_LENGTH);
        } else
        {
            CoverPath.Data = GetArcPath(currentValue);
        }

        StartPoints.Add(GetCirclePoint(CurrentVal));

        HistoryPath.Data = CreateLineSegments();
    }

    private PathGeometry CreateLineSegments()
    {
        var figure = new PathFigure()
        {
            StartPoint = GetCirclePoint(Min)
        };
        figure.Segments = new PathSegmentCollection();

        for (int i = 1; i < StartPoints.Count; i++)
        {
            figure.Segments.Add(new LineSegment(StartPoints[i]));
        }

        return new PathGeometry
        {
            Figures = new PathFigureCollection
            {
                figure
            }
        };
    }

    private PathGeometry GetArcPath(double value)
    {
        return new PathGeometry(new PathFigureCollection()
            {
                new PathFigure()
                {
                    StartPoint = GetCirclePoint(value),
                    Segments = new PathSegmentCollection()
                    {
                        new ArcSegment()
                        {
                            SweepDirection = SweepDirection.Clockwise,
                            Size = new Size(ArcRadius, ArcRadius),
                            IsLargeArc = IsLargeArc(value),
                            Point = GetCirclePoint(Max),
                        }
                    },
                }
            });
    }

    private Point GetCirclePoint(double value)
    {
        double percent = (Max - value) / (Max - Min);
        double angle = (-45.0) + (percent * (270));
        double radAngle = angle * (Math.PI / 180.0);
        double x = ViewRadius + ArcRadius * Math.Cos(radAngle);
        double y = ViewRadius - ArcRadius * Math.Sin(radAngle);

        return new Point(x, y);
    }

    private bool IsLargeArc(double currentVal)
    {
        // Return true if the arc from current to Max will be > 180 degrees
        // Since the overall shape is 270, this is when the value is 1/3 of the way from minimum to maximum
        // (For this example, it should occur at values of 33.333 and above)
        return currentVal < ((Max - Min) / 3) + Min ? true : false;
    }

    private void Advance_Clicked(object sender, EventArgs e)
    {
        var oldValue = CurrentVal;
        CurrentVal = Math.Min(CurrentVal + 1.1, Max);
        CreateArcAndLineSegment(oldValue, CurrentVal);
    }

    private void Reverse_Clicked(object sender, EventArgs e)
    {
        var oldValue = CurrentVal;
        CurrentVal = Math.Max(CurrentVal - 1.1, Min);
        CreateArcAndLineSegment(oldValue, CurrentVal);
    }
}