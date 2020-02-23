import React from 'react';

function TopLeaderboardLoader(props) {
    const {maxWidth} = props;
    return (
        <>
            <rect x="0" y="10" rx="5" ry="5" width={maxWidth} height="60"/>
            <rect x="0" y="75" rx="5" ry="5" width={maxWidth} height="60"/>
            <rect x="0" y="140" rx="5" ry="5" width={maxWidth} height="60"/>
        </>
    )
}

export default TopLeaderboardLoader;