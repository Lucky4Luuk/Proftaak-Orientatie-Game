uniform sampler2D texture;

uniform sampler2D shadowLayer;
uniform vec2 playerCoords;
uniform vec2 layerSize;

bool castRay(vec2 origin, vec2 endpoint) {
  for (int i = 0; i < floor(distance(origin, endpoint)); i++) {
    vec2 current = origin + (endpoint - origin) * (i / distance(origin, endpoint));
    bool hit = (texture2D(shadowLayer, current / layerSize).a > 0.0);
    if (hit) return true;
  }
  return false;
}

void main() {
  //gl_FragColor = vec4(1.0, vec2(0.0), 1.0);
  gl_FragColor = texture2D(texture, gl_TexCoord[0].xy);

  if (castRay(gl_TexCoord[0].xy * layerSize, playerCoords)) {
    gl_FragColor = vec4(vec3(0.0), 1.0);
  }

}
