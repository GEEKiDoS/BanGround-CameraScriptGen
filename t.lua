function IIF(expr, a, b)
    if expr then return a else return b end
end

local HALFPI = math.pi / 2;

local EaseLib = {
    [0] = function (p)
        return IIF(p < 1.0, 0, 1);
    end,
    [1] = function (p)
        return p;
    end,
    [2] = function (p)
        return p * p;
    end,
    [3] = function (p)
        return -(p * (p - 2));
    end,
    [4] = function (p)
        if p < 0.5 then
            return 2 * p * p;
        else
            return (-2 * p * p) + (4 * p) - 1;
        end
    end,
    [5] = function (p)
        return p * p * p;
    end,
    [6] = function (p)
        local f = (p - 1);
        return f * f * f + 1;
    end,
    [7] = function (p)
        if p < 0.5 then
            return 4 * p * p *p;
        else
            local f = ((2 * p) - 2);
            return 0.5 * f * f * f + 1;
        end
    end,
    [8] = function (p)
        return p * p * p * p;
    end,
    [9] = function (p)
        local f = (p - 1);
		return f * f * f * (1 - p) + 1;
    end,
    [10] = function (p)
        if p < 0.5 then
			return 8 * p * p * p * p;
		else
			local f = (p - 1);
			return -8 * f * f * f * f + 1;
        end
    end,
    [11] = function (p)
        return p * p * p * p * p;
    end,
    [12] = function (p)
        local f = (p - 1);
	    return f * f * f * f * f + 1;
    end,
    [13] = function (p)
        if p < 0.5 then
			return 16 * p * p * p * p * p;
		else
			local f = ((2 * p) - 2);
			return 0.5 * f * f * f * f * f + 1;
        end
    end,
    [14] = function (p)
        return math.sin((p - 1) * HALFPI) + 1;
    end,
    [15] = function (p)
        return math.sin(p * HALFPI);
    end,
    [16] = function (p)
        return 0.5 * (1 - math.cos(p * math.pi));
    end,
    [17] = function (p)
        return 1 - math.sqrt(1 - (p * p));
    end,
    [18] = function (p)
        return math.sqrt((2 - p) * p);
    end,
    [19] = function (p)
        if p < 0.5 then
			return 0.5 * (1 - math.sqrt(1 - 4 * (p * p)));
		else
			return 0.5 * (math.sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
        end
    end,
    [20] = function (p)
        return IIF(p == 0, p, 2 ^ (10 * (p - 1)));
    end,
    [21] = function (p)
        return IIF(p == 1, p , 1 - 2 ^ (-10 * p));
    end,
    [22] = function (p)
        if p == 0 or p == 1 then return p end;

        if p < 0.5 then
            return 0.5 * (2 ^ ((20 * p) - 10));
        else
            return -0.5 * (2 ^ ((-20 * p) + 10)) + 1;
        end
    end,
    [23] = function (p)
        return math.sin(13 * HALFPI * p) * (2 ^ (10 * (p - 1)));
    end,
    [24] = function (p)
        return math.sin(-13 * HALFPI * (p + 1)) * (2 ^ (-10 * p)) + 1;
    end,
    [25] = function (p)
        if p < 0.5 then
            return 0.5 * math.sin(13 * HALFPI * (2 * p)) * (2 ^ (10 * ((2 * p) - 1)));
        else
            return 0.5 * (math.sin(-13 * HALFPI * ((2 * p - 1) + 1)) * (2 ^ (-10 * (2 * p - 1))) + 2);
        end
    end,
    [26] = function (p)
        return p * p * p - p * math.sin(p * math.pi);
    end,
    [27] = function (p)
        local f = (1 - p);
		return 1 - (f * f * f - f * math.sin(f * math.pi));
    end,
    [28] = function (p)
        if p < 0.5 then
			local f = 2 * p;
			return 0.5 * (f * f * f - f * math.sin(f * math.pi));
		else
			local f = (1 - (2 * p - 1));
			return 0.5 * (1 - (f * f * f - f * math.sin(f * math.pi))) + 0.5;
        end
    end
};

EaseLib[30] = function (p)
    if p < 4 / 11.0 then
		return (121 * p * p) / 16.0;
	elseif p < 8 / 11.0 then
		return (363 / 40.0 * p * p) - (99 / 10.0 * p) + 17 / 5.0;
	elseif p < 9 / 10.0 then
		return (4356 / 361.0 * p * p) - (35442 / 1805.0 * p) + 16061 / 1805.0;
    else
        return (54 / 5.0 * p * p) - (513 / 25.0 * p) + 268 / 25.0;    
    end
end;
EaseLib[29] = function (p)
    return 1 - EaseLib[30](1 - p);     
end;
EaseLib[31] = function (p)
    if p < 0.5 then
		return 0.5 * EaseLib[29](p * 2);
	else
		return 0.5 * EaseLib[30](p * 2 - 1) + 0.5;
    end
end;

function Lerp(a, b, progress)
    return a + (b - a) * progress;
end

function UpdateObject(currentTime, object)
    -- there's a enter animation already :(
    if currentTime < 0 then
        return;
    end

    local reversed = false;

    if #object.KeyFrames < object.LastKeyFrame or object.LastKeyFrame < 1 then
        object.LastKeyFrame = 1;
    end

    if #object.KeyFrames == object.LastKeyFrame then
        return;
    end
    
    if object.KeyFrames[object.LastKeyFrame].Time > currentTime then
        reversed = true;
    end

    local firstKeyframe = 0;
    local nextKeyframe = 0;

    local targetIndex = 1;
    local step = -1;
    if not reversed then
        targetIndex = #object.KeyFrames;
        step = 1;
    end

    for i = object.LastKeyFrame, targetIndex, step do
        if object.KeyFrames[i].Time == currentTime then
            firstKeyframe = i;
            nextKeyframe = i;
            break;
        end

        if object.KeyFrames[i].Time < currentTime then
            firstKeyframe = i;

            if reversed then
                break;
            end
        end

        if object.KeyFrames[i].Time > currentTime then
            nextKeyframe = i;

            if not reversed then
                break;
            end
        end
    end

    if nextKeyframe == 0 or firstKeyframe == 0 or firstKeyframe == nextKeyframe then
        local idx = nextKeyframe;

        if nextKeyframe == 0 then idx = firstKeyframe end;
        
        local kf = object.KeyFrames[idx];
        object.LastKeyFrame = idx;

        object.Object:SetPosition(kf.X, kf.Y, kf.Z);
        object.Object:SetRotation(kf.Pitch, kf.Yaw, kf.Roll);

        if not object.IsCamera then
            object.Object:SetColor(kf.R, kf.G, kf.B, kf.A);
            object.Object:SetScale(kf.SX, kf.SY, kf.SZ);
        end
    else
        local kf1 = object.KeyFrames[firstKeyframe];
        local kf2 = object.KeyFrames[nextKeyframe];
        object.LastKeyFrame = firstKeyframe;

        local t = (currentTime - kf1.Time) / (kf2.Time - kf1.Time);

        -- Only lerp for now
        local func = EaseLib[kf2.Mode];

        object.Object:SetPosition(Lerp(kf1.X, kf2.X, func(t)), Lerp(kf1.Y, kf2.Y, func(t)), Lerp(kf1.Z, kf2.Z, func(t)));
        object.Object:SetRotation(Lerp(kf1.Pitch, kf2.Pitch, func(t)), Lerp(kf1.Yaw, kf2.Yaw, func(t)), Lerp(kf1.Roll, kf2.Roll, func(t)));

        if not object.IsCamera then
            object.Object:SetColor(Lerp(kf1.R, kf2.R, func(t)), Lerp(kf1.G, kf2.G, func(t)), Lerp(kf1.B, kf2.B, func(t)), Lerp(kf1.A, kf2.A, func(t)));
            object.Object:SetScale(Lerp(kf1.SX, kf2.SX, func(t)), Lerp(kf1.SY, kf2.SY, func(t)), Lerp(kf1.SZ, kf2.SZ, func(t)));
        end
    end
end
