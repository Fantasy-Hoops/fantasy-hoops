import React from "react";
import ContentLoader from "react-content-loader";

import './CustomLoader.css';

function CustomLoader(props) {
    const {children, maxWidth} = props;
    return (
        <div className="Loader__Container">
            <ContentLoader
                speed={2}
                width={maxWidth}
                height={200}
                viewBox={`0 0 ${maxWidth} 200`}
                backgroundColor="#f3f3f3"
                foregroundColor="#ecebeb"
            >
                {children}
            </ContentLoader>
        </div>
    );
}

export  default CustomLoader;