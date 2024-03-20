// MIT License
//
// Copyright (c) 2024 SPIN - Space Innovation
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace SPIN.Services;

public class AltitudeFromPressureService(IMediator mediator) : IRequestHandler<GetAltitudeRequest, GetAltitudeResponse>
{
    private static Pressure PressureAtSeaLevel { get; } = Pressure.FromHectopascals(1013.25);

    private readonly IMediator _mediator = mediator;

    public async Task<GetAltitudeResponse> Handle(GetAltitudeRequest request, CancellationToken cancellationToken)
    {
        GetPressureResponse pressureResponse = await _mediator.Send(GetPressureRequest.Instance);

        // H = (44330 * (1 - (P / P0)^(1/5.255)))
        double fraction = pressureResponse.Pressure / PressureAtSeaLevel;
        double altitudeInMeters = (44330 * (1 - Math.Pow(fraction, 1d / 5.255d)));

        GetAltitudeResponse response = new()
        {
            RequestId = request.RequestId,
            Altitude = Length.FromMeters(altitudeInMeters),
            Time = pressureResponse.Time
        };

        return response;
    }
}
